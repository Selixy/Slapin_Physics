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
        public State state {get{return collisionBuffer.state;}}
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
            bool isBounce;
            (velocity, hit, isBounce) = Move.Apply(gameObject, velocity);
            // Add collision to buffer if there is a collider
            if(hit.collider != null) {
                collisionBuffer.AddCollision(hit);
            }

            collisionBuffer.Update();

            Debug.Log("velocity: " + velocity);

        }

    }
}
