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
        private float velocityTrashhold = 0.01f;

        // Variables accessibles
        public State state {get{return collisionBuffer.state;}}
        public bool isBounce {get; internal set;}
        public Vector2 velocity{get; internal set;}
        public void AddVelocity(Vector2 addVelocity) { this.velocity += addVelocity; }
        public void SetVerticalVelocity(float velocity) { this.velocity = new Vector2(this.velocity.x, velocity); }
        public void SetVelocity(Vector2 velocity) { this.velocity = velocity; }

        // Constructor
        public Physic(GameObject gameObject, float velocityTrashhold = 0.01f)
        {
            // references et variables de definition
            this.gameObject = gameObject;
            this.velocityTrashhold = velocityTrashhold;

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
            if (velocity.magnitude > velocityTrashhold) {
                RaycastHit2D hit;
                (velocity, hit, isBounce) = Move.Apply(gameObject, velocity);
                // Add collision to buffer if there is a collider
                if(hit.collider != null) {
                    collisionBuffer.AddCollision(hit);
                }
            }

            collisionBuffer.Update();

        }
    }
}
