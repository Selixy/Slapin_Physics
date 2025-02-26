using UnityEngine;

namespace Physics
{
    public class Move
    {
        public GameObject gameObject;
        public Physic physic;

        public Move(Physic physic, GameObject gameObject)
        {
            this.gameObject = gameObject;
            this.physic = physic;
        }

        public void AddMove(Vector2 velocity)
        {
            Vector2 move = velocity * Time.deltaTime;
            Vector3 position = gameObject.transform.position;


            RaycastHit2D hit = BodyShapeCast.ShapeCast(move, gameObject);
            if (hit.collider != null)
            {
                Vector2 colliderSize = GetColliderSize() * 0.5f;

                move.x = hit.point.x - position.x + colliderSize.x * hit.normal.x;
                move.y = hit.point.y - position.y + colliderSize.y * hit.normal.y;

                physic.CollisionEvent(hit);
            }

            position.x += move.x;
            position.y += move.y;

            gameObject.transform.position = position;
        }

        public Vector2 GetColliderSize()
        {
            Collider2D collider = gameObject.GetComponent<Collider2D>();
            if (collider != null)
            {
                return collider.bounds.size;
            }
            return Vector2.zero;
        }
    }
}
