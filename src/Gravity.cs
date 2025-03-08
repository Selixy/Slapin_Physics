using UnityEngine;

namespace Physics
{
    public class Gravity
    {
        private float gravity = 9.81f;
        public float gravityFactor = 1f;
        public bool isGravityActive = true;

        // references
        private Physic physic;

        // constructor
        public Gravity(Physic physic)
        {
            this.physic = physic;
        }

        public void Update()
        {
            if (isGravityActive)
            {
                float gravityForce = -gravity * gravityFactor * Time.deltaTime;
                physic.AddVelocity(new Vector2(0, gravityForce));
                //Debug.Log("Gravity is active");
            }
            else {
                //Debug.Log("Gravity is not active");
            }
        }
    }
}