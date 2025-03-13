using UnityEngine;

namespace Physics
{
    public class Dumping
    {
        // dumping factor definition
        private float currentViscousDamping;

        // dumping factor definition
        private static float viscousAirDamping = 1f;
        private static float viscousWaterDamping = 1f;
        private static float dumpingBonk = 1f;

        // references
        private Physic physic;

        // constructor
        public Dumping(Physic physic)
        {
            this.physic = physic;
            currentViscousDamping = viscousAirDamping;
        }

        public void Update()
        {
            ViscousDamping();
        }

        public void DumpingBonk()
        {
            physic.velocity *= dumpingBonk;
        }

        private void ViscousDamping()
        {
            // apply viscous damping
            physic.velocity *= currentViscousDamping;
        }

        public void SetViscousDamping(int Indice = 0)
        {
            switch (Indice) {
                case 0:
                    currentViscousDamping = viscousAirDamping;
                    break;
                case 1:
                    currentViscousDamping = viscousWaterDamping;
                    break;
                default:
                    currentViscousDamping = viscousAirDamping;
                    break;
            }
        }
    }
}