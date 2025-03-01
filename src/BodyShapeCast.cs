using UnityEngine;
using System.Collections;

namespace Physics
{
    public static class BodyShapeCast
    {
        /// <summary>
        /// Affiche en mode débogage le BoxCast du BoxCollider2D associé au GameObject.
        /// </summary>
        public static void DebugShapeCast(Vector2 castVector, Color debugColor, float debugDuration = -1f, GameObject gameObject = null)
        {
            BoxCollider2D box = gameObject != null ? gameObject.GetComponent<BoxCollider2D>() : null;
            if (box == null)
            {
                Debug.LogWarning("Aucun BoxCollider2D trouvé sur le GameObject fourni.");
                return;
            }

            if (debugDuration < 0f)
                debugDuration = Time.deltaTime;

            // Calcul du centre d'origine en tenant compte de l'offset
            Vector2 origCenter = (Vector2)box.transform.position + box.offset;
            // Calcul du centre de destination
            Vector2 destCenter = origCenter + castVector;

            // Dessiner le contour du box d'origine
            DrawBoxAt(origCenter, box.size, box.transform.eulerAngles.z, debugColor, debugDuration);
            // Dessiner le contour du box à la destination
            DrawBoxAt(destCenter, box.size, box.transform.eulerAngles.z, debugColor, debugDuration);
            // Dessiner les segments reliant les coins entre les deux positions
            DrawBoxConnectingLines(origCenter, destCenter, box.size, box.transform.eulerAngles.z, debugColor, debugDuration);

            // Effectue le cast optimisé en ignorant le GameObject d'origine
            RaycastHit2D hit = PerformOptimizedCast(box, castVector, box.gameObject);
            if (hit.collider != null)
            {
                Debug.Log($"Optimized Cast a touché : {hit.collider.name} à la distance {hit.distance}");
            }
            else
            {
                Debug.Log("Optimized Cast n'a touché aucun collider.");
            }
        }

        /// <summary>
        /// Effectue un BoxCast sur le BoxCollider2D du GameObject et retourne le premier hit (en ignorant le GameObject d'origine).
        /// </summary>
        public static RaycastHit2D ShapeCast(Vector2 castVector, GameObject gameObject)
        {
            BoxCollider2D box = gameObject != null ? gameObject.GetComponent<BoxCollider2D>() : null;
            if (box == null)
            {
                Debug.LogWarning("Aucun BoxCollider2D trouvé sur le GameObject fourni.");
                return new RaycastHit2D();
            }

            RaycastHit2D hit = PerformOptimizedCast(box, castVector, box.gameObject);

            return hit;
        }

        /// <summary>
        /// Effectue le BoxCast optimisé et retourne le premier hit dont le GameObject n'est pas celui à ignorer.
        /// </summary>
        private static RaycastHit2D PerformOptimizedCast(BoxCollider2D box, Vector2 castVector, GameObject ignoreObject)
        {
            // Calcul des informations de base
            float distance = castVector.magnitude;
            Vector2 direction = castVector.normalized;
            float angle = box.transform.eulerAngles.z;

            // On récupère la marge
            float margin = Physic.marginDetection;

            // Convertir box.bounds.center (Vector3) en Vector2, puis ajouter la marge dans la direction du cast
            Vector2 castOrigin = (Vector2)box.bounds.center + direction * margin;

            // Réduire la taille du box de 2 fois la marge pour garder un décalage symétrique
            Vector2 castSize = new Vector2(
                box.size.x - 2f * margin,
                box.size.y - 2f * margin
            );

            // Réduire la distance de la même valeur pour ne pas pousser trop le cast
            float castDistance = distance - margin;
            if (castDistance < 0f)
            {
                castDistance = 0f;
            }

            // Empêche la taille de devenir négative au cas où la marge serait supérieure
            // à la moitié de la dimension d'origine
            castSize.x = Mathf.Max(castSize.x, 0f);
            castSize.y = Mathf.Max(castSize.y, 0f);

            // Effectue le BoxCast avec les nouvelles dimensions et positions
            RaycastHit2D[] hits = Physics2D.BoxCastAll(castOrigin, castSize, angle, direction, castDistance);
            return GetFirstValidHit(hits, ignoreObject);
        }

        /// <summary>
        /// Retourne le premier hit de la liste dont le GameObject n'est pas celui à ignorer.
        /// </summary>
        private static RaycastHit2D GetFirstValidHit(RaycastHit2D[] hits, GameObject ignoreObject)
        {
            foreach (RaycastHit2D hit in hits)
            {
                if (hit.collider != null && hit.collider.gameObject != ignoreObject)
                    return hit;
            }
            return new RaycastHit2D();
        }

        /// <summary>
        /// Dessine le contour d'un BoxCollider2D à une position donnée.
        /// </summary>
        private static void DrawBoxAt(Vector2 center, Vector2 size, float angleDegrees, Color color, float duration)
        {
            float angleRad = angleDegrees * Mathf.Deg2Rad;
            Vector2 halfSize = size / 2;
            Vector2 right = new Vector2(Mathf.Cos(angleRad), Mathf.Sin(angleRad));
            Vector2 up = new Vector2(-Mathf.Sin(angleRad), Mathf.Cos(angleRad));

            Vector2 topRight = center + right * halfSize.x + up * halfSize.y;
            Vector2 topLeft = center - right * halfSize.x + up * halfSize.y;
            Vector2 bottomRight = center + right * halfSize.x - up * halfSize.y;
            Vector2 bottomLeft = center - right * halfSize.x - up * halfSize.y;

            Debug.DrawLine(topRight, topLeft, color, duration);
            Debug.DrawLine(topLeft, bottomLeft, color, duration);
            Debug.DrawLine(bottomLeft, bottomRight, color, duration);
            Debug.DrawLine(bottomRight, topRight, color, duration);
        }

        /// <summary>
        /// Dessine des lignes reliant les coins d'un BoxCollider2D entre deux positions.
        /// </summary>
        private static void DrawBoxConnectingLines(Vector2 origCenter, Vector2 destCenter, Vector2 size, float angleDegrees, Color color, float duration)
        {
            float angleRad = angleDegrees * Mathf.Deg2Rad;
            Vector2 halfSize = size / 2;
            Vector2 right = new Vector2(Mathf.Cos(angleRad), Mathf.Sin(angleRad));
            Vector2 up = new Vector2(-Mathf.Sin(angleRad), Mathf.Cos(angleRad));

            Vector2 origTopRight = origCenter + right * halfSize.x + up * halfSize.y;
            Vector2 origTopLeft = origCenter - right * halfSize.x + up * halfSize.y;
            Vector2 origBottomRight = origCenter + right * halfSize.x - up * halfSize.y;
            Vector2 origBottomLeft = origCenter - right * halfSize.x - up * halfSize.y;

            Vector2 destTopRight = destCenter + right * halfSize.x + up * halfSize.y;
            Vector2 destTopLeft = destCenter - right * halfSize.x + up * halfSize.y;
            Vector2 destBottomRight = destCenter + right * halfSize.x - up * halfSize.y;
            Vector2 destBottomLeft = destCenter - right * halfSize.x - up * halfSize.y;

            Debug.DrawLine(origTopRight, destTopRight, color, duration);
            Debug.DrawLine(origTopLeft, destTopLeft, color, duration);
            Debug.DrawLine(origBottomRight, destBottomRight, color, duration);
            Debug.DrawLine(origBottomLeft, destBottomLeft, color, duration);
        }
    }
}
