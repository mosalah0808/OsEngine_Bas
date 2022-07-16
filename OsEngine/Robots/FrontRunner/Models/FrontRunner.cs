using OsEngine.Entity;
using OsEngine.OsTrader.Panels;
using OsEngine.OsTrader.Panels.Tab;
using OsEngine.Robots.FrontRunner.View;
using OsEngine.Robots.FrontRunner.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OsEngine.Robots.FrontRunner
{
    public class FrontRunnerBot : BotPanel
    {
        public FrontRunnerBot(string name, StartProgram startProgram) : base(name, startProgram)
        {
            TabCreate(BotTabType.Simple);
            _tab = TabsSimple[0];
            _tab.MarketDepthUpdateEvent += _tab_MarketDepthUpdateEvent;
            //_tab.PositionOpeningFailEvent += _tab_PositionOpeningFailEvent;
        }

       

        #region =======================FIELDS=========================

        public decimal BigVolume = 12000;
        public int Offset = 1;
        public int Take= 10;
        public decimal Lot = 2;
        public Position Position = null;
        private BotTabSimple _tab;
        public PositionStateType CurrentPos = PositionStateType.None;
        public decimal LotOpened = 0;
        public decimal PriceOpened;
        public decimal TakeOpened;
        public decimal VarMargin;
        public decimal ProfitCurrent;
        #endregion
        #region =======================Properties=========================

        public Edit Edit
        {
            get => _edit;
            set
            {
                _edit = value;
                if (_edit == Edit.Stop && Position != null && Position.State == PositionStateType.Opening)
                {
                    _tab.CloseAllOrderInSystem();
                }
                
            }

        }

        private Edit _edit = Edit.Stop;

        #endregion

        #region ===================Method================================
        //private void _tab_PositionOpeningFailEvent(Position pos)
        //{
        //    Position = null;
        //}

     
        private void _tab_MarketDepthUpdateEvent(MarketDepth marketDepth)
        {
           if (Edit == Edit.Stop) 
            { 
                return;
            }
            
            if (marketDepth.SecurityNameCode != _tab.Securiti.Name)
             {
                return;
             }

            List<Position> positions = _tab.PositionsOpenAll;

            if (positions != null && positions.Count > 0)
            {
                
                foreach (Position pos in positions)
                {
                    Position = pos;
                    if (Position.State == PositionStateType.Open)
                    {

                    
                        if (pos.Direction == Side.Sell)
                        {
                            decimal takePrice = pos.EntryPrice - Take * _tab.Securiti.PriceStep;
                            _tab.CloseAtProfit(Position, takePrice, takePrice);
                        }
                        else if (pos.Direction == Side.Buy)
                        {
                            decimal takePrice = pos.EntryPrice + Take * _tab.Securiti.PriceStep;
                            _tab.CloseAtProfit(Position, takePrice, takePrice);
                        }

                        CurrentPos = pos.State;
                        LotOpened = pos.OpenVolume;
                        PriceOpened = pos.EntryPrice;
                        TakeOpened = pos.ProfitOrderPrice;
                        VarMargin = pos.ProfitPortfolioPunkt;
                        ProfitCurrent += VarMargin;
                    }
            }
            }

           

            for (int i = 0; i < marketDepth.Asks.Count; i++)
            {
                if (marketDepth.Asks[i].Ask >= BigVolume && Position==null )
                {
                    decimal price = marketDepth.Asks[i].Price - Offset * _tab.Securiti.PriceStep;
                    VarMargin = 0;
                    Position = _tab.SellAtLimit(Lot, price);
                    Thread.Sleep(1000);
                    if (Position.State != PositionStateType.Open && Position.State != PositionStateType.Opening)
                    {
                        Position = null;
                    }
                    
                }

                if (Position!= null && marketDepth.Asks[i].Price == Position.EntryPrice && marketDepth.Asks[i].Ask < BigVolume/2)
                {
                    if (Position.State == PositionStateType.Open)
                    {
                        _tab.CloseAtMarket(Position, Position.OpenVolume);
                        //Position = null;
                    }
                    else if (Position.State == PositionStateType.Opening)
                    {
                        _tab.CloseAllOrderInSystem();
                        Position = null;
                    }
                }
                else if (Position != null
                           && Position.State == PositionStateType.Opening
                           && marketDepth.Asks[i].Ask >= BigVolume
                           && marketDepth.Asks[i].Price < Position.EntryPrice + Offset * _tab.Securiti.PriceStep)
                {
                    _tab.CloseAllOrderInSystem();
                    Position = null;
                    break;

                }
            }

            
            
            for (int i = 0; i < marketDepth.Bids.Count; i++)
            {
                if (marketDepth.Bids[i].Bid >= BigVolume && Position == null )
                {
                    decimal price = marketDepth.Bids[i].Price + Offset * _tab.Securiti.PriceStep;
                    VarMargin = 0;
                    Position = _tab.BuyAtLimit(Lot, price);
                    Thread.Sleep(1000);
                    if (Position.State != PositionStateType.Open && Position.State != PositionStateType.Opening)
                    {
                        Position = null;
                    }
                    
                }

                if (Position != null 
                    && marketDepth.Bids[i].Price == Position.EntryPrice- Offset * _tab.Securiti.PriceStep
                    && marketDepth.Bids[i].Bid < BigVolume / 2)
                {
                    if (Position.State == PositionStateType.Open)
                    {
                        _tab.CloseAtMarket(Position, Position.OpenVolume);
                        //Position = null;
                    }
                    else if (Position.State == PositionStateType.Opening)
                    {
                        _tab.CloseAllOrderInSystem();
                        Position = null;
                    }
                   
                } else if (Position != null 
                            && Position.State == PositionStateType.Opening
                            && marketDepth.Bids[i].Bid >= BigVolume
                            && marketDepth.Bids[i].Price > Position.EntryPrice - Offset * _tab.Securiti.PriceStep)
                  {
                    _tab.CloseAllOrderInSystem();
                    Position = null;
                        break;

                  }
            }

        }


        public override string GetNameStrategyType()
        {
            return "FrontRunnerBot";
        }

        public override void ShowIndividualSettingsDialog()
        {
            FrontRunnerUi window = new FrontRunnerUi(this);
            window.Show();
        }

        #endregion
    }
}
