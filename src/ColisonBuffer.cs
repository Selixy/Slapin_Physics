using System.Collections.Generic;
using UnityEngine;

namespace Physics
{
    public class CollisionBuffer
    {
        // definition des coefficients de friction et adherence
        private static float defaultFriction = 0f;
        private static float defaultAdherence = 0f;
        private static float iceFriction = 0f;
        private static float iceAdherence = 0f;


        public State state{get; private set;}

        private List<CollisionData> collisionList;
        private GameObject gameObject;


        // Propriété pour accéder en lecture seule à la liste des collisions
        public List<CollisionData> CollisionList => collisionList;

        // Constructeur
        public CollisionBuffer(GameObject gameObject)
        {
            collisionList = new List<CollisionData>();
            this.gameObject = gameObject;

            // definition des coefficients de friction et adherence
            defaultFriction = StaticDefinition.defaultFriction;
            defaultAdherence = StaticDefinition.defaultAdherence;
            iceFriction = StaticDefinition.iceFriction;
            iceAdherence = StaticDefinition.iceAdherence;
        }


        private (float friction, float adherence) GetFrictionAdherenceFromTag(string tag)
        {
            switch(tag) {
                case "Ice":
                    return (iceFriction, iceAdherence);
                default:
                    return (defaultFriction, defaultAdherence);
            }
        }

        public void AddCollision(RaycastHit2D hit)
        {
            CollisionData collision = new CollisionData();
            collision.Normal = hit.normal;
            collision.Friction = GetFrictionAdherenceFromTag(hit.collider.tag).friction;
            collision.Adherence = GetFrictionAdherenceFromTag(hit.collider.tag).adherence;

            collisionList.Add(collision);
        }


        private void SetState()
        {
            bool foundGround = false;
            bool foundWall = false;
            float groundThreshold = 0.1f;
            float wallThreshold = 0.1f;

            // Vérifier la présence d'une collision orientée vers le haut
            foreach(CollisionData collision in collisionList)
            {
                if(collision.Normal.y > 1f - groundThreshold) {
                    foundGround = true;
                    break;
                }
            }

            // Si aucune collision orientée vers le haut n'a été trouvée,
            // alors on cherche une collision orientée horizontalement
            if(!foundGround) {
                foreach(CollisionData collision in collisionList)
                {
                    if(Mathf.Abs(collision.Normal.x) > 1f - wallThreshold) {
                        foundWall = true;
                        break;
                    }
                }
            }

            // Déterminer l'état en fonction des collisions trouvées
            if(foundGround) {
                state = State.OnGround;
            }
            else if(foundWall) {
                state = State.OnWall;
            }
            else {
                state = State.InAir;
            }
        }

        // Update : teste toutes les collisions et les supprime si elles n'existent plus
        public void Update()
        {
            // Parcourir la liste en sens inverse
            for (int i = collisionList.Count - 1; i >= 0; i--)
            {
                CollisionData collision = collisionList[i];

                // Calculer la direction opposée à la normale de la collision
                Vector2 collisionDirection = collision.Normal * -1f;
                collisionDirection = collisionDirection.normalized * 0.01f;

                // Effectuer un raycast depuis l'objet dans cette direction
                RaycastHit2D hit = ShapCast.Cast(gameObject, collisionDirection);

                // Si le raycast ne touche plus aucun collider, supprimer la collision
                if (hit.collider == null) {
                    collisionList.RemoveAt(i);
                }
                else {
                    (float friction, float adherence) = GetFrictionAdherenceFromTag(hit.collider.tag);
                    collision.Friction = friction;
                    collision.Adherence = adherence;
                    collision.Normal = hit.normal;
                }
            }

            SetState();
            Debug.Log("State: " + state);
        }
    }
}
