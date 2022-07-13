using OsEngine.Commands;
using OsEngine.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace OsEngine.Robots.FrontRunner.View
{
    public class VM : BaseVM
    {
        public VM(FrontRunnerBot bot)
        {
            _bot = bot;
        }

        #region =============FIELDS===================

        private FrontRunnerBot _bot;
        #endregion

        #region =============Properties===============

        public decimal BigVolume
        {
            get => _bot.BigVolume;
            set
            {
                _bot.BigVolume = value;
                OnPropertyChanged(nameof(BigVolume));
            }
        }
        
        
        
        public int Offset
        {
            get => _bot.Offset;
            set
            {
                _bot.Offset = value;
                OnPropertyChanged(nameof(Offset));
            }
        }
       

        public int Take
        {
            get => _bot.Take;
            set
            {
                _bot.Take = value;
                OnPropertyChanged(nameof(Take));
            }
        }
        

        public decimal Lot
        {
            get => _bot.Lot;
            set
            {
                _bot.Lot = value;
                OnPropertyChanged(nameof(Lot));
            }
        }
        


        public Edit Edit
        {
            get => _bot.Edit;
            set
            {
                _bot.Edit = value;
                OnPropertyChanged(nameof(Edit));
            }
        }

        public PositionStateType CurrentPos
        {
            get => _bot.CurrentPos;
            set
            {
                _bot.CurrentPos = value;
                OnPropertyChanged(nameof(CurrentPos));
            }
        }

        public decimal LotOpened
        {
            get => _bot.LotOpened;
            set
            {
                _bot.LotOpened = value;
                OnPropertyChanged(nameof(LotOpened));
            }
        }

        public decimal PriceOpened
        {
            get => _bot.PriceOpened;
            set
            {
                _bot.PriceOpened = value;
                OnPropertyChanged(nameof(PriceOpened));
            }
        }

        public decimal TakeOpened
        {
            get => _bot.TakeOpened;
            set
            {
                _bot.TakeOpened = value;
                OnPropertyChanged(nameof(TakeOpened));
            }
        }

        public decimal VarMargin
        {
            get => _bot.VarMargin;
            set
            {
                _bot.VarMargin = value;
                OnPropertyChanged(nameof(VarMargin));
            }
        }

        public decimal ProfitCurrent
        {
            get => _bot.ProfitCurrent;
            set
            {
                _bot.ProfitCurrent = value;
                OnPropertyChanged(nameof(ProfitCurrent));
            }
        }

        #endregion

        #region =============Commands===============

        private DelegateCommand commandStart;

        public ICommand CommandStart
        {
            get
            {
                if (commandStart==null)
                {
                    commandStart = new DelegateCommand(Start);
                }
                return commandStart;
            }
        }


        #endregion

        #region =============Methods===============

        private void Start(object obj)
        {
            if (Edit == Edit.Start)
            {
                Edit = Edit.Stop;
            }
            else
            {
                Edit = Edit.Start;
            }
        }


        #endregion
    }

    public enum Edit
    {
        Start,
        Stop
    }
}
