using UnityEngine;

namespace Physics
{
    public static class Move
    {
        private static float bounceTolerance = 100f;
        public static (Vector2 velocity, RaycastHit2D hit, bool isBounce) Apply(GameObject gameObject, Vector2 velocity)
        {
            // Variables
            bool isBounce = false;

            // Récupérer la position actuelle
            Vector2 position;
            position.x = gameObject.transform.position.x;
            position.y = gameObject.transform.position.y;
            // Calculer la vitesse pour ce frame
            Vector2 frameVelocity = velocity * Time.deltaTime;

            // Effectuer la ShapCast
            Vector2 originOffset = new Vector2(0.0f, 0.0f);
            RaycastHit2D hit = ShapCast.Cast(gameObject, frameVelocity, originOffset, 0.0f);
            // Vérifier que la collision concerne bien une surface entravant le déplacement
            if (hit.collider != null && Vector2.Dot(frameVelocity, hit.normal) < 0) {
                // Calculer le déplacement jusqu'au premier contact
                Vector2 displacementBeforeCollision = frameVelocity.normalized * hit.distance;

                // Calculer le déplacement restant si aucune collision n'était détectée
                Vector2 displacementAfterCollision = frameVelocity - displacementBeforeCollision;

                // Si la vitesse est supérieure à 10, réfléchir le déplacement restant par rapport à la normale
                if (frameVelocity.magnitude > bounceTolerance) {
                    displacementAfterCollision *= StaticDefinition.dumpingBonk;
                    displacementAfterCollision = Vector2.Reflect(displacementAfterCollision, hit.normal);
                    isBounce = true;
                }
                else {
                    Vector2 tangent = new Vector2(hit.normal.y, hit.normal.x);
                    // Projection trigonométrique
                    displacementAfterCollision = Vector2.Dot(displacementAfterCollision, tangent.normalized) * tangent.normalized;
 
                }
                // Combiner le déplacement avant collision et le déplacement ajusté
                frameVelocity = displacementBeforeCollision + displacementAfterCollision;

                // Mettre à jour la vélocité à appliquer
                if (Time.deltaTime != 0) {
                    velocity = frameVelocity / Time.deltaTime;
                }
            }

            // Appliquer la mise à jour de la position
            position += frameVelocity;
            gameObject.transform.position = new Vector3(position.x, position.y, gameObject.transform.position.z);


            return (velocity, hit, isBounce);
        }

        private static (Vector2 displacementAfterCollision, Vector2 displacementBeforeCollision) RecursiveCorection(
            GameObject gameObject, 
            Vector2 displacementAfterCollision, 
            Vector2 displacementBeforeCollision, 
            int recursionCount = 0)
        {
            // Vérifier si on a atteint la limite de récursion ou si le déplacement restant est négligeable
            if (recursionCount >= 1)
            {
                return (Vector2.zero, displacementBeforeCollision);
            }

            RaycastHit2D hit = ShapCast.Cast(gameObject, displacementAfterCollision, displacementBeforeCollision, 0.0f);
            if (hit.collider != null && Vector2.Dot(displacementAfterCollision, hit.normal) < 0) {
                // Calculer le déplacement jusqu'au point de collision
                displacementBeforeCollision += displacementAfterCollision.normalized * hit.distance;
                displacementAfterCollision -= displacementAfterCollision.normalized * hit.distance;

                // Projection sur la tangente (calcul de la tangente : ici on échange les composantes)
                Vector2 tangent = new Vector2(hit.normal.y, hit.normal.x);
                displacementAfterCollision = Vector2.Dot(displacementAfterCollision, tangent.normalized) * tangent.normalized;

                // Appel récursif en incrémentant le compteur
                return RecursiveCorection(gameObject, displacementAfterCollision, displacementBeforeCollision, recursionCount + 1);
            }
            else {
                return (displacementAfterCollision, displacementBeforeCollision);
            }
        }
    }
}