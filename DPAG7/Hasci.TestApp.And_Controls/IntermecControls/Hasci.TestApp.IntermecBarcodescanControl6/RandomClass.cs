using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace Hasci.TestApp.IntermecBarcodeScanControls6
{
    class RandomClass
    {
        int _iSeed;
//        double dRnd;
        Random _random;
        double _treshold = 0.3;
        public RandomClass()
        {
            init();
        }
        public RandomClass(double treshold)
        {
            _treshold = treshold;
            init();
        }
        private void init()
        {
            _iSeed = (int)DateTime.Now.Ticks;
            _random = new Random(_iSeed);
        }
        public bool getRandom()
        {
            double dbl=_random.NextDouble();
            if (dbl < _treshold)
                return true;
            else
                return false;
        }
    }
}
