using System;
using UnityEngine;

namespace Physics
{
    public static class Move
    {
        // Événement pour transmettre un message "bonk"
        public static event Action<GameObject, Vector2, RaycastHit2D> Impact;
        public static event Action<GameObject> Bonk;

        public static (Vector2 velocity, RaycastHit2D hit, bool isBounce) Apply(GameObject obj, Vector2 velocity)
        {

            // Calculer la vitesse pour ce frame
            Vector2 frameVelocity = velocity * Time.deltaTime;

            // Effectuer le ShapeCast (vérifiez que ShapCast.Cast est bien défini)
            Vector2 originOffset = Vector2.zero;
            RaycastHit2D hit = ShapCast.Cast(obj, frameVelocity, originOffset, 0.0f);

            // Vérifier que la collision concerne bien une surface entravant le déplacement
            bool isBounce = false; // On initialise la variable isBounce à false
            if (hit.collider != null && Vector2.Dot(frameVelocity, hit.normal) < 0)
            {

                // Calculer le déplacement jusqu'au premier contact
                Vector2 displacementBeforeCollision = frameVelocity.normalized * hit.distance;

                // Calculer le déplacement restant si aucune collision n'était détectée
                Vector2 displacementAfterCollision = frameVelocity - displacementBeforeCollision;

                // Calculer la force de l'impact
                float forceImpact = Mathf.Abs(Vector2.Dot(velocity, hit.normal));
                float angleImpact = 180f - Mathf.Abs(Vector2.Angle(velocity.normalized, hit.normal));

                if (forceImpact > StaticDefinition.bounceStrengthTolerance 
                && angleImpact < StaticDefinition.bounceAngleTolerance) {
                    // Cas où la force est forte et l'angle est bon (bounce)
                    // Dévier le déplacement restant en appelant Bounce avec le hit de collision
                    (displacementAfterCollision, displacementBeforeCollision) = Bounce(obj, displacementAfterCollision, displacementBeforeCollision, hit);

                    Bonk?.Invoke(obj);
                    isBounce = true;
                }
                else {
                    // Cas où la force est faible (pas de bounce, correction du déplacement)
                    // Calculer la tangente perpendiculaire à la normale
                    Vector2 tangent = new Vector2(hit.normal.y, -hit.normal.x);
                    displacementAfterCollision = Vector2.Dot(displacementAfterCollision, tangent.normalized) * tangent.normalized;

                    // Correction récursive du déplacement restant
                    (displacementAfterCollision, displacementBeforeCollision) = RecursiveCorrection(obj, displacementAfterCollision, displacementBeforeCollision);
                }

                // Mettre à jour les vélocités à appliquer
                (velocity, frameVelocity) = UpdateVelocity(velocity, displacementBeforeCollision, displacementAfterCollision);
                Impact?.Invoke(obj, velocity, hit);
            }

            // Appliquer la mise à jour de la position
            ApplyPosition(obj, frameVelocity);

            return (velocity, hit, isBounce);
        }

        // Méthode pour corriger recursivement les déplacements en cas de collision à faible force
        private static (Vector2 displacementAfterCollision, Vector2 displacementBeforeCollision) RecursiveCorrection(
            GameObject obj,
            Vector2 displacementAfterCollision,
            Vector2 displacementBeforeCollision,
            int recursionCount = 0)
        {
            // Limiter la récursion
            if (recursionCount >= StaticDefinition.maxCorrectionRecursion) {
                return (Vector2.zero, displacementBeforeCollision);
            }

            RaycastHit2D hit = ShapCast.Cast(obj, displacementAfterCollision, displacementBeforeCollision, 0.0f);
            if (hit.collider != null && Vector2.Dot(displacementAfterCollision, hit.normal) < 0) {
                displacementBeforeCollision += displacementAfterCollision.normalized * hit.distance;
                displacementAfterCollision -= displacementAfterCollision.normalized * hit.distance;

                // Calcul de la tangente perpendiculaire à la normale
                Vector2 tangent = new Vector2(hit.normal.y, -hit.normal.x);
                displacementAfterCollision = Vector2.Dot(displacementAfterCollision, tangent.normalized) * tangent.normalized;

                return RecursiveCorrection(obj, displacementAfterCollision, displacementBeforeCollision, recursionCount + 1);
            }
            else {
                return (displacementAfterCollision, displacementBeforeCollision);
            }
        }

        // Méthode pour dévier recursivement le déplacement en cas de forte collision (bounce)
        private static (Vector2 displacementAfterCollision, Vector2 displacementBeforeCollision) Bounce(
            GameObject obj,
            Vector2 displacementAfterCollision,
            Vector2 displacementBeforeCollision,
            RaycastHit2D hit,
            int recursionCount = 0)
        {
            // Appliquer le dumping (réduction de vitesse)
            displacementAfterCollision *= StaticDefinition.dumpingBonk;


            // Reduir la magnitude corespondent a la tengent de la normale
            Vector2 tangent = new Vector2(hit.normal.y, -hit.normal.x);
            Vector2 tangentComponent = Vector2.Dot(displacementAfterCollision, tangent) * tangent;
            Vector2 normalComponent = displacementAfterCollision - tangentComponent;
            tangentComponent *= StaticDefinition.tangenteDumpingBonk;
            displacementAfterCollision = normalComponent + tangentComponent;


            // Réfléchir le déplacement restant par rapport à la normale de collision
            displacementAfterCollision = Vector2.Reflect(displacementAfterCollision, hit.normal);

            // Calculer la force (norme du vecteur après réflexion)
            float force = displacementAfterCollision.magnitude;
            Vector2 direction = displacementAfterCollision.normalized;

            // Modifier la direction en interpolant vers la normale (pour un effet de "bonk")
            Vector2 newDirection = Vector2.Lerp(direction, hit.normal, StaticDefinition.normalisBonk);
            displacementAfterCollision = newDirection * force;

            // Vérifier récursivement si la nouvelle direction entraîne une nouvelle collision
            // On utilise une constante (maxBounceRecursion) définie dans StaticDefinition pour limiter la récursion
            if (recursionCount < StaticDefinition.maxBounceRecursion)
            {
                // Effectuer un ShapeCast pour la nouvelle trajectoire
                RaycastHit2D newHit = ShapCast.Cast(obj, displacementAfterCollision, displacementBeforeCollision, 0.0f);
                if (newHit.collider != null && Vector2.Dot(displacementAfterCollision, newHit.normal) < 0)
                {
                    // Calculer la portion du déplacement avant la nouvelle collision
                    Vector2 bounceDisplacementBefore = displacementAfterCollision.normalized * newHit.distance;
                    displacementBeforeCollision += bounceDisplacementBefore;
                    displacementAfterCollision -= bounceDisplacementBefore;

                    // Appel récursif pour corriger le déplacement en fonction de la nouvelle collision
                    return Bounce(obj, displacementAfterCollision, displacementBeforeCollision, newHit, recursionCount + 1);
                }
            }

            // Retourner les déplacements corrigés après bounce
            return (displacementAfterCollision, displacementBeforeCollision);
        }

        // Méthode pour mettre à jour les vélocités
        private static (Vector2 velocity, Vector2 frameVelocity) UpdateVelocity(Vector2 velocity, Vector2 displacementBeforeCollision, Vector2 displacementAfterCollision)
        {
            // Calculer la vitesse pour cette frame
            Vector2 frameVelocity = displacementBeforeCollision + displacementAfterCollision;

            // Calculer la nouvelle vélocité en fonction du deltaTime
            if (Time.deltaTime != 0)
            {
                return (frameVelocity / Time.deltaTime, frameVelocity);
            }
            return (velocity, frameVelocity);
        }

        // Méthode pour appliquer la position
        private static void ApplyPosition(GameObject obj, Vector2 frameVelocity)
        {
            // Récupérer la position actuelle de l'objet
            Vector2 position = new Vector2(obj.transform.position.x, obj.transform.position.y);

            // Appliquer le déplacement calculé
            position += frameVelocity;
            obj.transform.position = new Vector3(position.x, position.y, obj.transform.position.z);
        }
    }
}