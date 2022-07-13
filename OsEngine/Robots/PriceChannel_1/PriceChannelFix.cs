using OsEngine.Entity;
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
            Mode = CreateParameter("Mode", "Off", new[] { "Off", "On"});
            Lot = CreateParameter("Lot", 10, 5, 20, 1);
            Risk = CreateParameter("Risk", 1m, 0.2m, 3.0m, 0.1m);

            _pc = IndicatorsFactory.CreateIndicatorByName("PriceChannelFix",name+ "PriceChannelFix",false);
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
