using UnityEngine;

namespace Physics
{
    public class Dumping
    {
        // dumping factor definition
        private static float dumpingBase = 1f;
        private static float dumpingIce = 1f;
        private static float dumpingSticky = 1f;
        private static float dumpingBonk = 1f;

        // references
        private Physic physic;

        // constructor
        public Dumping(Physic physic)
        {
            this.physic = physic;
        }

        public void Update()
        {
            physic.velocity *= dumpingBase;
        }
        public void DumpingBonk()
        {
            physic.velocity *= dumpingBonk;
        }
    }
}