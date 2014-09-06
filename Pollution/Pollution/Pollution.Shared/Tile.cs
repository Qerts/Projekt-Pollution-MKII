using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI.Xaml;

namespace Pollution 
{
    public class Tile 
    {
        public Tile()
        {
            
        }

        public enum TileType 
        {
            TestTile,
            BlankTile,
            ImageTile,
            StringTile,
            StatusTile_SO2,
            StatusTile_O3,
            StatusTile_CO,
            StatusTile_PM10,
            StatusTile_NO2,
            MainStatusTile,
            ButtonTile_MenuPanel,
            ButtonTile_DataPanel,
            ButtonTile_MapPanel,
        }
    }
}
