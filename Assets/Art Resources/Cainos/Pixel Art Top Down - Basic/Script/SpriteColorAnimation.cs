using UnityEngine;

namespace Art_Resources.Cainos.Pixel_Art_Top_Down___Basic.Script
{
    //animate the sprite color base on the gradient and time
    public class SpriteColorAnimation : MonoBehaviour
    {
        public Gradient gradient;
        public float time;

        private SpriteRenderer sr;
        private float timer;

        private void Start()
        {
            timer = time * Random.value;
            sr = GetComponent<SpriteRenderer>();
        }

        private void Update()
        {
            if (sr)
            {
                timer += Time.deltaTime;
                if (timer > time) timer = 0.0f;

                sr.color = gradient.Evaluate(timer / time);
            }
        }
    }
}
