using UnityEngine;

namespace Physics
{
    public static class ShapCast
    {
        // Marge utilisée pour réduire la taille effective en tangent et décaler l'origine.
        private static float margin = 0.05f;

        public static RaycastHit2D Cast(GameObject obj, Vector2 vectorCast)
        {
            BoxCollider2D boxCollider = obj.GetComponent<BoxCollider2D>();
            if (boxCollider == null) {
                Debug.LogWarning("Aucun BoxCollider2D trouvé sur l'objet, impossible d'effectuer le BoxCast.");
                return new RaycastHit2D();
            }

            Vector2 objectPosition = (Vector2)obj.transform.position;
            Vector2 adjustedOrigin = GetAdjustedOrigin(boxCollider, objectPosition, vectorCast);
            Vector2 effectiveSize = GetEffectiveSize(boxCollider, vectorCast);

            float angle = obj.transform.eulerAngles.z;
            Vector2 direction = vectorCast.normalized;
            float distance = vectorCast.magnitude;

            // Debug visuel : trace la ligne complète du cast en vert
            Debug.DrawLine(adjustedOrigin, adjustedOrigin + vectorCast, Color.green, 0.1f);

            // Exécution du BoxCastAll
            RaycastHit2D[] hits = Physics2D.BoxCastAll(adjustedOrigin, effectiveSize, angle, direction, distance);

            // Recherche du meilleur hit externe (hors composants de l'objet)
            RaycastHit2D bestHit = new RaycastHit2D();
            foreach (RaycastHit2D h in hits)
            {
                if (h.collider != null && h.collider.gameObject != obj) {
                    if (bestHit.collider == null || h.distance < bestHit.distance) {
                        bestHit = h;
                    }
                }
            }

            return bestHit;
        }


        private static Vector2 GetAdjustedOrigin(BoxCollider2D collider, Vector2 objectPosition, Vector2 vectorCast)
        {
            Vector2 origin = objectPosition + collider.offset;
            return origin + vectorCast.normalized * (margin * 0.5f);
        }

        private static Vector2 GetEffectiveSize(BoxCollider2D collider, Vector2 vectorCast)
        {
            Vector2 effectiveSize = collider.size;
            if (Mathf.Abs(vectorCast.x) >= Mathf.Abs(vectorCast.y)) {
                effectiveSize.y = Mathf.Max(0, effectiveSize.y - margin);
            }
            else {
                effectiveSize.x = Mathf.Max(0, effectiveSize.x - margin);
            }
            return effectiveSize;
        }
    }
}