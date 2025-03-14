using UnityEngine;

namespace Physics
{
    public class Dumping
    {
        // internal variables
        private float currentViscousDamping;

        // references
        private Physic physic;

        // constructor
        public Dumping(Physic physic)
        {
            this.physic = physic;
            currentViscousDamping = StaticDefinition.viscousAirDamping;
        }

        public void Update()
        {
            ViscousDamping();
        }

        public void DumpingBonk()
        {
            physic.velocity *= StaticDefinition.dumpingBonk;
        }

        private void ViscousDamping()
        {
            // Ajuste l'amortissement viscous selon le temps écoulé
            physic.velocity *= Mathf.Pow(currentViscousDamping, Time.deltaTime);
        }


        public void SetViscousDamping(int Indice = 0)
        {
            switch (Indice) {
                case 0:
                    currentViscousDamping = StaticDefinition.viscousAirDamping;
                    break;
                case 1:
                    currentViscousDamping = StaticDefinition.viscousWaterDamping;
                    break;
                default:
                    currentViscousDamping = StaticDefinition.viscousAirDamping;
                    break;
            }
        }
    }
}