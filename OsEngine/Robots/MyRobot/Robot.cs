using OsEngine.Entity;
using OsEngine.OsTrader.Panels;
using OsEngine.OsTrader.Panels.Tab;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace OsEngine.Robots.MyRobot
{
    public class Robot : BotPanel
    {
        public Robot(string name, StartProgram startProgram) : base(name, startProgram)
        {
            TabCreate(BotTabType.Simple);
            _tab = TabsSimple[0];
            paramString =  CreateParameter("Mode", "Edit", new[] { "Mode", "Edit" });
           
            
            _risk = CreateParameter("Risk %", 0.2m, 0.1m, 10m, 0.1m);
            _profitKoef = CreateParameter("Koef Profit", 2m, 0.1m, 10m, 0.1m);
            _countDownCandles = CreateParameter("Count Down Candles", 3, 1, 5, 1);
            _koefVolume = CreateParameter("Koef Volume", 2m, 2m,  10m, 0.5m);
            _countCandles = CreateParameter("Count Candles", 10, 5, 50, 1);

            _tab.CandleFinishedEvent += _tab_CandleFinishedEvent;
            _tab.PositionOpeningSuccesEvent += _tab_PositionOpeningSuccesEvent;
            _tab.PositionClosingSuccesEvent += _tab_PositionClosingSuccesEvent;
        }

        


        #region =========================FIELDS ====================================================
        private BotTabSimple _tab;
        public StrategyParameterString paramString;


        /// <summary>
        /// риск на сделку
        /// </summary>
        public StrategyParameterDecimal _risk;
        /// <summary>
        /// во сколько раз теик больше риска
        /// </summary>
        public StrategyParameterDecimal _profitKoef;
        /// <summary>
        /// Количество падающих свечей перед падающим разворотом
        /// </summary>
        public StrategyParameterInt _countDownCandles;
        /// <summary>
        /// во сколько раз обьем превышает средний
        /// </summary>
        public StrategyParameterDecimal _koefVolume;
        /// <summary>
        /// кол-во свечей для вычисления среднего обьема
        /// </summary>
        public StrategyParameterInt _countCandles;
        /// <summary>
        /// средний обьем
        /// </summary>
        public decimal _averageVolume;
        public int _punkts;
        public decimal _lowCandle;

        #endregion

        #region ================================METHODS=======================================================

        private void _tab_CandleFinishedEvent(List<Candle> candles)
        {
           if (candles.Count < _countDownCandles.ValueInt + 1 || candles.Count < _countCandles.ValueInt + 1)
            {
                return;
            }
            Candle candle = candles[candles.Count - 1];
            List<Position> positions = _tab.PositionsOpenAll;
            if (positions.Count == 1)
            {
                if (candle.Close- positions[0].EntryPrice > positions[0].EntryPrice-_lowCandle)
                {
                    _lowCandle = positions[0].EntryPrice;
                    _tab.CloseAtStop(positions[0], _lowCandle, _lowCandle - 100 * _tab.Securiti.PriceStep);
                }
               
            }
            else if (positions.Count > 1)
            { 
                return;
            }

            _averageVolume = 0;

            for (int i = candles.Count - 2; i > candles.Count - 2 - _countCandles.ValueInt; i--)
            {
                _averageVolume += candles[i].Volume;
            }

            _averageVolume /= _countCandles.ValueInt;

        

            if (candle.Close < (candle.High+candle.Low)/2 ||
                candle.Volume < _averageVolume* _koefVolume.ValueDecimal)
            {
                return;
            }

            for (int i = candles.Count - 2; i > candles.Count - 2 - _countDownCandles.ValueInt; i--)
            {
                if (candles[i].Close > candles[i].Open)
                {
                    return;
                }
            }

             _punkts = (int)((candle.Close - candle.Low) / _tab.Securiti.PriceStep);
            if (_punkts < 5)
            {
                return;
            }
            decimal amountStop = _punkts * _tab.Securiti.PriceStepCost;
            decimal amountRisk = _tab.Portfolio.ValueBegin * _risk.ValueDecimal / 100;
            decimal _volume = amountRisk/ amountStop;

            decimal go = 10000;
            if (_tab.Securiti.Go > 1)
            {
                go = _tab.Securiti.Go;
            }

            decimal maxLot = _tab.Portfolio.ValueBegin / go;
            if (maxLot > _volume)
            {
                _lowCandle = candle.Low;
                _tab.BuyAtMarket(_volume);
            }

            
           
        }

        private void _tab_PositionOpeningSuccesEvent(Position position)
        {
            decimal priceTake = position.EntryPrice + _punkts * _profitKoef.ValueDecimal;
            _tab.CloseAtProfit(position, priceTake, priceTake);
           
            _tab.CloseAtStop(position, _lowCandle, _lowCandle -100*_tab.Securiti.PriceStep);
          
        }
        private void _tab_PositionClosingSuccesEvent(Position position)
        {
            LoadCSV(position);
        }


        private void LoadCSV(Position position)
        {
            if (!File.Exists(@"Engine\trades.csv"))
            {
                string header = "; Позиция; Символ; Лоты; Изменение / Максимум Лотов;" +
                  "Исполнение входа; Сигнал входа; Бар входа; Дата входа;" +
                  " Время входа; Цена входа; Комиссия входа; Исполнение выхода;" +
                  "Сигнал выхода; Бар выхода; Дата выхода; Время выхода; Цена выхода;" +
                  "Комиссия выхода; Средневзвешенная цена входа; П / У; П / У сделки;" + "" +
                  "П / У с одного лота; Зафиксированная П/ У; Открытая П/ У; Продолж. (баров);" + "" +
                  " Доход / Бар; Общий П/ У;% изменения; MAE; MAE %; MFE; MFE %";

                using (StreamWriter writer = new StreamWriter(@"Engine\trades.csv", false))
                {
                    writer.WriteLine(header);
                    writer.Close();
                }
            }

            using (StreamWriter writer = new StreamWriter(@"Engine\trades.csv", true))
            {
                string str = ";;;;;;;;" + position.TimeOpen.ToShortDateString();
                str += ";" + position.TimeOpen.TimeOfDay;
                str += ";;;;;;;;;;;;;;" + position.ProfitPortfolioPunkt + ";;;;;;;;";
                writer.WriteLine(str);
                writer.Close();
            }


        }
        public override string GetNameStrategyType()
        {
            return "Robot";
        }

        public override void ShowIndividualSettingsDialog()
        {
            Window1 window = new Window1(this);
            window.ShowDialog();
          
        }
        #endregion
    }
}
