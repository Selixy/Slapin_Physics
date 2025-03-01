using UnityEngine;

namespace Physics
{
    public class Physic
    {
        public GameObject gameObject;
        public Move move;
        public Gravity gravity;
        public PhysicState physicState;


        public Vector2 normalWall;
        public Vector2 velocity;
        public bool isStatic;

        public static float marginDetection = 0.01f;
        public static float velocityMargin = 0.005f;
        public static float dampingBounce = 0.9f;

        public Move GetInstanceMove() { return move; }
        public Gravity GetInstanceGravity() { return gravity; }

        // Constructor
        public Physic(GameObject gameObject)
        {
            this.gameObject = gameObject;
            move = new Move(this, gameObject);
            gravity = new Gravity(this, gameObject.GetComponent<MonoBehaviour>());
            gravity.StartGravity();
        }

        public void Update()
        {
            if (Mathf.Abs(velocity.y) > velocityMargin || Mathf.Abs(velocity.x) > velocityMargin)
            {
                isStatic = false;
                move.AddMove(velocity);
            }
            else
            {
                isStatic = true;
            }
            if (physicState != PhysicState.IsInAir)
            {
                StateManager.StateControl(this);
            }
        }

        public void CollisionEvent(RaycastHit2D hit)
        {
            if (hit.collider != null)
            {
                physicState = StateManager.SetState(hit.normal);
                if (physicState == PhysicState.IsGrounded)
                {
                    gravity.StopGravity();
                }
                else if (physicState == PhysicState.IsOnWall)
                {
                    normalWall = hit.normal;
                }
            }
        }
    }
}
