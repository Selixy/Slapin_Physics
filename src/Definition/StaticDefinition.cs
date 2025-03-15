namespace Physics
{
    public static class StaticDefinition
    {
        // definition des coefficients de friction et adherence
        public static float VerticalFactor = 0.65f;

        public static float defaultFriction = 0.05f;
        public static float defaultAdherence = 1f;

        public static float iceFriction = 0.02f;
        public static float iceAdherence = 1f;


        // definition des coefficients de dumping
        public static float viscousAirDamping = 0.9995f;
        public static float viscousWaterDamping = 0.99f;

        public static float dumpingBonk = 0.95f;

        // definition de la gravité
        public static float gravity = 70f;

        // definition des Facteurs d'inertie
        public static float inertiaFactor = 1f;
    }
}