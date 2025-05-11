using System.Collections;
using Gyemong.GameSystem.Object;
using UnityEngine;

namespace Gyemong.GameSystem.Map.Puzzle.TemplePuzzle
{
    public class TempleTile : InteractableObject
    {
        public TempleTileData templeTileData;

        private bool isRotatable = false;
        private bool isRotating = false;
        public bool iswalked = false;

        private float rotationAmount = -90f;
        private float rotationSpeed = 0.5f;

        public bool up;
        public bool down;
        public bool left;
        public bool right;

        private void Start()
        {
            isRotatable = templeTileData.isRotatable;

            up = templeTileData.up;
            down = templeTileData.down;
            left = templeTileData.left;
            right = templeTileData.right;

            if (isRotatable)
            {
                FirstRandomRotate();
            }
        }

        protected override void OnInteraction(Collider2D collision)
        {
            BallMovement ball = FindObjectOfType<BallMovement>();
            bool isBallMove = ball.isMoving;

            if (collision.CompareTag("Player") && !isRotating && isRotatable && !isBallMove)
            {
                StartCoroutine(RotateTile());
            }
        }

        public IEnumerator RotateTile()
        {
            isRotating = true;

            float time = 0f;
            Quaternion startRotation = transform.rotation;
            Quaternion endRotation = Quaternion.Euler(0f, 0f, transform.rotation.eulerAngles.z + rotationAmount);

            while (time < rotationSpeed)
            {
                transform.rotation = Quaternion.Slerp(startRotation, endRotation, time / rotationSpeed);
                time += Time.deltaTime;
                yield return null;
            }
            transform.rotation = endRotation;

            Rotate();
        }

        private void FirstRandomRotate()
        {
            int randomRotation = Random.Range(0, 4) * 90;
            for (int i = 0; i < randomRotation / 90; i++)
            {
                Rotate();
            }
            transform.rotation = Quaternion.Euler(0, 0, -randomRotation);
        }

        void Rotate()
        {
            bool temp;

            temp = up;
            up = left;
            left = down;
            down = right;
            right = temp;

            isRotating = false;
        }
    }
}
