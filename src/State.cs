using UnityEngine;

namespace Physics
{
    public static class StateManager
    {
        public static PhysicState SetState(Vector2 normal)
        {
            if (normal.y > 0.5f)
            {
                return PhysicState.IsGrounded;
            }
            else
            {
                return PhysicState.IsInAir;
            }
        }

        public static void StateControl(Physic physic)
        {
            PhysicState state = physic.physicState;
            float margin = Physic.marginDetection;
            Vector2 normalWall = physic.normalWall;
            GameObject gameObject = physic.gameObject;

            if (state == PhysicState.IsGrounded)
            {
                Vector2 ray = Vector2.down * margin;
                RaycastHit2D hit = BodyShapeCast.ShapeCast(ray, gameObject);
                if (hit.collider == null)
                {
                    StateRecalculate(physic, margin, gameObject);
                }
            }
            else if (state == PhysicState.IsOnWall)
            {
                if (normalWall.x > 0)
                {
                    Vector2 ray = Vector2.right * margin;
                    RaycastHit2D hit = BodyShapeCast.ShapeCast(ray, gameObject);
                    if (hit.collider == null)
                    {
                        StateRecalculate(physic, margin, gameObject);
                    }
                }
                else if (normalWall.x < 0)
                {
                    Vector2 ray = Vector2.left * margin;
                    RaycastHit2D hit = BodyShapeCast.ShapeCast(ray, gameObject);
                    if (hit.collider == null)
                    {
                        StateRecalculate(physic, margin, gameObject);
                    }
                }
            }
            else if (state == PhysicState.IsOnCeiling)
            {
                Vector2 ray = Vector2.up * margin;
                RaycastHit2D hit = BodyShapeCast.ShapeCast(ray, gameObject);
                if (hit.collider == null)
                {
                    StateRecalculate(physic, margin, gameObject);
                }
            }
        }

        private static void StateRecalculate(Physic physic, float margin, GameObject gameObject)
        {
            // Vérification vers le bas
            Vector2 rayDown = Vector2.down * margin;
            RaycastHit2D hitDown = BodyShapeCast.ShapeCast(rayDown, gameObject);
            if (hitDown.collider != null)
            {
                physic.physicState = PhysicState.IsGrounded;
                return;
            }

            // Vérification vers la droite
            Vector2 rayRight = Vector2.right * margin;
            RaycastHit2D hitRight = BodyShapeCast.ShapeCast(rayRight, gameObject);
            if (hitRight.collider != null)
            {
                physic.physicState = PhysicState.IsOnWall;
                return;
            }

            // Vérification vers la gauche
            Vector2 rayLeft = Vector2.left * margin;
            RaycastHit2D hitLeft = BodyShapeCast.ShapeCast(rayLeft, gameObject);
            if (hitLeft.collider != null)
            {
                physic.physicState = PhysicState.IsOnWall;
                return;
            }

            // Vérification vers le haut
            Vector2 rayUp = Vector2.up * margin;
            RaycastHit2D hitUp = BodyShapeCast.ShapeCast(rayUp, gameObject);
            if (hitUp.collider != null)
            {
                physic.physicState = PhysicState.IsOnCeiling;
                return;
            }

            // Si aucune collision n'est détectée, l'état est en l'air
            physic.physicState = PhysicState.IsInAir;
        }
    }
}
