using UnityEngine;

namespace Physics
{
    public class Physic
    {
        // Instances
        private Gravity gravity;
        private Friction friction;
        private Dumping dumping;
        public CollisionBuffer collisionBuffer {get; private set;}

        // references
        public GameObject gameObject;

        // Variables accessibles
        public Vector2 velocity{get; internal set;}
        public void AddVelocity(Vector2 addVelocity) { this.velocity += addVelocity; }


        // Constructor
        public Physic(GameObject gameObject)
        {
            // references
            this.gameObject = gameObject;

            //AddRigidbody2D();

            // Instances
            collisionBuffer = new CollisionBuffer(gameObject);
            friction = new Friction(this);
            gravity = new Gravity(this);
            dumping = new Dumping(this);
        }

        public void Update()
        {
            friction.Update();
            dumping.Update();
            gravity.Update();

            // Apply velocity
            RaycastHit2D hit;
            (velocity, hit) = Move.Apply(gameObject, velocity);
            // Add collision to buffer if there is a collider
            if(hit.collider != null) {
                collisionBuffer.AddCollision(hit);
            }

            collisionBuffer.Update();

            Debug.Log("velocity: " + velocity);

        }

        private void AddRigidbody2D() {
            Rigidbody2D rb = gameObject.GetComponent<Rigidbody2D>();
            if (rb == null) {
                rb = gameObject.AddComponent<Rigidbody2D>();
                rb.bodyType = RigidbodyType2D.Kinematic;
            }
        }
    }
}