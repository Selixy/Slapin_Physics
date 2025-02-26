using UnityEngine;
using System.Collections;

namespace Physics
{
    public class Gravity
    {
        public float gravity = 9.81f;
        public float gravityScale = 1f;

        private Coroutine gravityCoroutine;

        public Physic physic;
        public MonoBehaviour behaviour;





        public Gravity(Physic physic, MonoBehaviour behaviour)
        {
            this.physic = physic;
            this.behaviour = behaviour;
        }


        public void StartGravity()
        {
            gravityCoroutine = behaviour.StartCoroutine(ApplyGravity());
        }
        public void StopGravity()
        {
            if (gravityCoroutine != null)
            {
                behaviour.StopCoroutine(gravityCoroutine);
                gravityCoroutine = null;
                physic.velocity.y = 0;
            }
        }



        private IEnumerator ApplyGravity()
        {
            while (true)
            {
                physic.velocity.y -= gravity * Time.deltaTime * gravityScale;
                yield return null;
            }
        }
    }
}