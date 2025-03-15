using UnityEngine;

namespace Physics
{
    public static class ShapCast
    {

        public static RaycastHit2D Cast(GameObject obj, Vector2 vectorCast, Vector2 OriginOffset, float margin = 0f)
        {
            BoxCollider2D boxCollider = obj.GetComponent<BoxCollider2D>();
            if (boxCollider != null) {
                return BoxCast(boxCollider, vectorCast, OriginOffset, margin);
            }

            CircleCollider2D circleCollider = obj.GetComponent<CircleCollider2D>();
            if (circleCollider != null) {
                Debug.LogWarning("CircleCast non implémenté");
                return new RaycastHit2D();
            }

            CapsuleCollider2D capsuleCollider = obj.GetComponent<CapsuleCollider2D>();
            if (capsuleCollider != null) {
                Debug.LogWarning("CapsuleCast non implémenté");
                return new RaycastHit2D();
            }

            PolygonCollider2D polygonCollider = obj.GetComponent<PolygonCollider2D>();
            if (polygonCollider != null) {
                Debug.LogWarning("PolygonCast non implémenté");
                return new RaycastHit2D();
            }

            Debug.LogWarning("Aucun BoxCollider2D trouvé sur l'objet, impossible d'effectuer le BoxCast.");
            return new RaycastHit2D();
        }

        private static RaycastHit2D BoxCast(BoxCollider2D boxCollider, Vector2 vectorCast, Vector2 Origin, float margin = 0f)
        {
            Vector2 objectPosition = (Vector2)boxCollider.transform.position;
            Vector2 adjustedOrigin = GetAdjustedOrigin(boxCollider, objectPosition, vectorCast) + Origin;
            Vector2 effectiveSize = GetEffectiveSize(boxCollider, vectorCast, margin);

            float angle = boxCollider.transform.eulerAngles.z;
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
                if (h.collider != null && h.collider.gameObject != boxCollider.gameObject) {
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
            return origin + vectorCast.normalized *  0.025f;
        }

        private static Vector2 GetEffectiveSize(BoxCollider2D collider, Vector2 vectorCast, float margin = 0f)
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