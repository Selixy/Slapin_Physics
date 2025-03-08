using UnityEngine;

namespace Physics
{
    public class Physic
    {
        // Instances
        private Gravity gravity;
        private Friction friction;
        private Dumping dumping;

        // references
        public GameObject gameObject{get; private set;}

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
            dumping = new Dumping(this);
            gravity = new Gravity(this);
            friction = new Friction(this);
        }

        public void Update()
        {
            friction.Update();
            dumping.Update();
            gravity.Update();

            // Apply velocity
            RaycastHit2D hit;
            (velocity, hit) = Move.Apply(gameObject, velocity);
            if (hit.normal.y > 0.9) {
                gravity.isGravityActive = false;
            }
            //Move.ResolveRepulsion(gameObject);

            Debug.DrawRay(gameObject.transform.position, velocity, Color.red);
            //Debug.Log(velocity);
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
