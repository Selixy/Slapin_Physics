namespace Physics
{
    public static class StaticDefinition
    {
        // definition des coefficients de friction et adherence
        public static float VerticalFactor = 1f;

        public static float defaultFriction = 15f;
        public static float defaultAdherence = 1f;

        public static float iceFriction = 10f;
        public static float iceAdherence = 1f;


        // definition des coefficients de dumping
        public static float viscousAirDamping = 0.9995f;
        public static float viscousWaterDamping = 0.99f;


        // definition des coefficients du rebond
        public static float dumpingBonk = 0.40f;
        public static float tangenteDumpingBonk = 0.10f;
        public static float normalisBonk = 0.3f;

        public static float bounceStrengthTolerance = 50f;
        public static float bounceAngleTolerance = 80f;


        // definition des Limites des recursivités
        public static int maxBounceRecursion = 3;
        public static int maxCorrectionRecursion = 2;


        // definition de la gravités
        public static float gravity = 70f;

        // definition des Facteurs d'inertie
        public static float inertiaFactor = 1f;

    }
}