
using OsEngine.Entity;
using OsEngine.OsTrader.Panels;
using OsEngine.OsTrader.Panels.Tab;

namespace OsEngine.Robots.MyRobot_2
{
    public class VM : BaseVM
    {
        public VM(Robot_2 robot)
        {
            _robot = robot;
            Mode = robot.paramString.ValueString;
            Risk = robot._risk.ValueDecimal;
            ProfitKoef = robot._profitKoef.ValueDecimal;
            CountDownCandles = robot._countDownCandles.ValueInt;
            KoefVolume = robot._koefVolume.ValueDecimal;
            CountCandles = robot._countCandles.ValueInt;
        }

        private Robot_2 _robot;

        public decimal Risk
        {
            get => _robot._risk.ValueDecimal;
            set
            {
                _risk = value;
                _robot._risk.ValueDecimal = _risk;
                OnPropertyChanged(nameof(Risk));
            }

        }
        private decimal _risk;

        public decimal ProfitKoef
        {
            get => _robot._profitKoef.ValueDecimal;
            set
            {
                _profitKoef = value;
                _robot._profitKoef.ValueDecimal = _profitKoef;
                OnPropertyChanged(nameof(ProfitKoef));
            }

        }
        private decimal _profitKoef;

        public int CountDownCandles
        {
            get => _robot._countDownCandles.ValueInt;
            set
            {
                _countDownCandles = value;
                _robot._countDownCandles.ValueInt = _countDownCandles;
                OnPropertyChanged(nameof(CountDownCandles));
            }

        }
        private int _countDownCandles;

        public string Mode
        {
            get => _robot.paramString.ValueString;
            set
            {
                _mode = value;
                _robot.paramString.ValueString = _mode;
                OnPropertyChanged(nameof(Mode));
            }

        }
        private string _mode;

        
         public decimal KoefVolume
        {
            get => _robot._koefVolume.ValueDecimal;
            set
            {
                _koefVolume = value;
                _robot._koefVolume.ValueDecimal = _koefVolume;
                OnPropertyChanged(nameof(KoefVolume));
            }

        }
        private decimal _koefVolume;

        public int CountCandles
        {
            get => _robot._countCandles.ValueInt;
            set
            {
                _countCandles = value;
                _robot._countCandles.ValueInt = _countCandles;
                OnPropertyChanged(nameof(CountCandles));
            }

        }
        private int _countCandles;
     
    }
}