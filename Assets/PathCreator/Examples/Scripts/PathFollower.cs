using UnityEngine;

namespace PathCreation.Examples
{
    // Moves along a path at constant speed.
    // Depending on the end of path instruction, will either loop, reverse, or stop at the end of the path.
    public class PathFollower : MonoBehaviour
    {
        public PathCreator pathCreator;
        public EndOfPathInstruction endOfPathInstruction;
        public float speed = 5;
        [SerializeField] bool rotateAroundPath;
        public bool allowZRot;
        private Vector3 currentPosition;
        private Vector3 lastPosition;
        private Vector3 nextPosition;
        public float anglenextpos{get;private set;}
        public bool RotateAround{get=>rotateAroundPath;set=> rotateAroundPath=value;}
        float distanceTravelled;

        void Start() {
            if (pathCreator != null)
            {
                // Subscribed to the pathUpdated event so that we're notified if the path changes during the game
                pathCreator.pathUpdated += OnPathChanged;
            }
        }

        void Update()
        {
            if (pathCreator != null)
            {
                distanceTravelled += speed * Time.deltaTime;
                transform.position = pathCreator.path.GetPointAtDistance(distanceTravelled, endOfPathInstruction);
                if(rotateAroundPath)//transform.rotation = pathCreator.path.GetRotationAtDistance(distanceTravelled, endOfPathInstruction);
                {
                    if (allowZRot)
                    {
                        //transform.rotation = pathCreator.path.GetRotationAtDistance(distanceTravelled, endOfPathInstruction);
                        currentPosition = transform.position;
                        if (currentPosition != lastPosition)
                        {
                            nextPosition.x = (currentPosition.x * 2) - lastPosition.x;
                            nextPosition.y = (currentPosition.y * 2) - lastPosition.y;
                            Vector3 direction = nextPosition - transform.position;
                            anglenextpos = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                            transform.rotation = Quaternion.Euler(0f, 0f, anglenextpos + 90+70);
                        }
                        lastPosition = currentPosition;
                    }
                }
            }
        }

        // If the path changes during the game, update the distance travelled so that the follower's position on the new path
        // is as close as possible to its position on the old path
        void OnPathChanged() {
            distanceTravelled = pathCreator.path.GetClosestDistanceAlongPath(transform.position);
        }
    }
}