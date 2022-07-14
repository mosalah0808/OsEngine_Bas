﻿using OsEngine.Entity;
using OsEngine.Indicators;
using OsEngine.OsTrader.Panels;
using OsEngine.OsTrader.Panels.Tab;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OsEngine.Robots.PriceChannel_1
{
    class PriceChannelFix : BotPanel
    {

        public PriceChannelFix(string name, StartProgram startProgram) : base(name, startProgram)
        {
            TabCreate(BotTabType.Simple);
            _tab = TabsSimple[0];
            LenghtUp = CreateParameter("Lenght Channel Up", 12, 5, 80, 2);
            LenghtDown = CreateParameter("Lenght Channel Down", 12, 5, 80, 2);
            Mode = CreateParameter("Mode", "Off", new[] { "Off", "On", "OnlyLong", "OnlyShort", "OnlyClosePosition" });
            Lot = CreateParameter("Lot", 10, 5, 20, 1);
            Risk = CreateParameter("Risk", 1m, 0.2m, 3.0m, 0.1m);

            _pc = IndicatorsFactory.CreateIndicatorByName("PriceChannel", name + "PriceChannel",false);
            _pc.ParametersDigit[0].Value = LenghtUp.ValueInt;
            _pc.ParametersDigit[1].Value = LenghtDown.ValueInt;
            _pc = (Aindicator)_tab.CreateCandleIndicator(_pc,"Prime");
            _pc.Save();
            _tab.CandleFinishedEvent += _tab_CandleFinishedEvent;

        }

       
        #region =======================FIELDS=========================
        private BotTabSimple _tab;
        private Aindicator _pc;
        private StrategyParameterInt LenghtUp;
        private StrategyParameterInt LenghtDown;
        private StrategyParameterString Mode;
        private StrategyParameterInt Lot;
        private StrategyParameterDecimal Risk;

        #endregion

        #region ===================Method================================
        private void _tab_CandleFinishedEvent(List<Candle> candles)
        {
            if (Mode.ValueString == "Off")
            {
                return;
            }

            if (_pc.DataSeries[0].Values==null|| _pc.DataSeries[1].Values == null||
                _pc.DataSeries[0].Values.Count< LenghtUp.ValueInt + 1||
                 _pc.DataSeries[1].Values.Count < LenghtDown.ValueInt + 1)
            {
                return;
            }

            Candle candle = candles[candles.Count - 1];
            decimal lastUp = _pc.DataSeries[0].Values[_pc.DataSeries[0].Values.Count - 2];
            decimal lastDown = _pc.DataSeries[1].Values[_pc.DataSeries[1].Values.Count - 2];
            List<Position> positions = _tab.PositionsOpenAll;

            if (Mode.ValueString == "OnlyClosePosition")
            {
                return;
            }

            if (Mode.ValueString != "OnlyShort") //long
            {
                if (candle.Close > lastUp && candle.Open < lastUp && positions.Count == 0)
                {
                    decimal riskMoney = _tab.Portfolio.ValueBegin * Risk.ValueDecimal / 100;
                    decimal costPriceStep = _tab.Securiti.PriceStepCost;
                    costPriceStep = 1;
                    decimal steps = (lastUp - lastDown) / _tab.Securiti.PriceStep;
                    decimal lot = riskMoney / (steps * costPriceStep);

                   // _tab.BuyAtMarket((int)lot);
                    _tab.BuyAtMarket(Lot.ValueInt);
                }
            }

            if (Mode.ValueString != "OnlyLong") //short
            {
                if (candle.Close < lastDown && candle.Open > lastDown && positions.Count == 0)
                {
                    decimal riskMoney = _tab.Portfolio.ValueBegin * Risk.ValueDecimal / 100;
                    decimal costPriceStep = _tab.Securiti.PriceStepCost;
                    costPriceStep = 1;
                    decimal steps = (lastUp - lastDown) / _tab.Securiti.PriceStep;
                    decimal lot = riskMoney / (steps * costPriceStep);

                   // _tab.SellAtMarket((int)lot);
                    _tab.SellAtMarket(Lot.ValueInt);
                }
            }

            if (positions.Count > 0)
            {
                Trailing(positions);
            }
        }
        
        private void Trailing(List<Position> positions)
        {
            decimal lastDown = _pc.DataSeries[1].Values.Last();
            decimal lastUp = _pc.DataSeries[0].Values.Last();
            foreach (Position pos in positions)
            {
                if (pos.State == PositionStateType.Open)
                {
                    if (pos.Direction == Side.Buy)
                    {
                        _tab.CloseAtTrailingStop(pos, lastDown, lastDown - 100 * _tab.Securiti.PriceStep);
                    }
                    if (pos.Direction == Side.Sell)
                    {
                        _tab.CloseAtTrailingStop(pos, lastUp, lastUp + 100 * _tab.Securiti.PriceStep);
                    }
                }
            }
        }
        public override string GetNameStrategyType()
        {
            return nameof(PriceChannelFix);
        }

        public override void ShowIndividualSettingsDialog()
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
