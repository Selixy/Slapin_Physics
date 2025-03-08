using UnityEngine;

namespace Physics
{
    public static class Move
    {
        public static (Vector2 velocity, RaycastHit2D hit) Apply(GameObject gameObject, Vector2 velocity)
        {
            // get position
            Vector2 position;
            position.x = gameObject.transform.position.x;
            position.y = gameObject.transform.position.y;
            // set frame velocity
            Vector2 frameVelocity = velocity * Time.deltaTime;

            // ShapCast
            RaycastHit2D hit = ShapCast.Cast(gameObject, frameVelocity);
            if (hit.collider != null) {
                frameVelocity = frameVelocity.normalized * hit.distance;
                velocity = frameVelocity;
            }

            // apply velocity
            position += frameVelocity;

            // apply position
            gameObject.transform.position = new Vector3(position.x, position.y, gameObject.transform.position.z);

            return (velocity, hit);
        }
    }
}