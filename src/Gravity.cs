using UnityEngine;

namespace Physics
{
    public class Gravity
    {
        private float gravity = 70f;
        public float gravityFactor = 1f;
        public bool isGravityActive = true;

        // references
        private Physic physic;

        // constructor
        public Gravity(Physic physic)
        {
            this.physic = physic;
            this.gravity = StaticDefinition.gravity;
        }

        public void Update()
        {
            if (isGravityActive && physic.collisionBuffer.state != State.OnGround)
            {
                float gravityForce = -gravity * gravityFactor * Time.deltaTime;
                physic.AddVelocity(new Vector2(0, gravityForce));
            }
        }
    }
}