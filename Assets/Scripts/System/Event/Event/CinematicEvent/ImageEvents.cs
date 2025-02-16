using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public abstract class ImageEvent : CinematicEvent
{
    private Image _image;
    protected Image Image
    {
        get
        {
            if (_image == null)
            {
                _image = EffectManager.Instance.transform.Find("EventImage").GetComponent<Image>();
            }
            return _image;
        }
    }
}

public class ShowImageEvent : ImageEvent
{
    [SerializeField] private Sprite sprite;
    [SerializeField] private float time = 0.2f;
    private const float DELTA_TIME = 0.02f;
    public override IEnumerator Execute(EventObject eventObject = null)
    {
        float defaultScale = Image.transform.localScale.x;
        float minScale = defaultScale * 0.8f;
        float detalScale = (defaultScale - minScale) / (time / DELTA_TIME);
        Image.sprite = sprite;
        Color color = Image.color;
        color.a = 0;
        float timer = 0;
        while (timer < time)
        {
            timer += DELTA_TIME;
            Image.transform.localScale = new Vector3(Image.transform.localScale.x + detalScale, Image.transform.localScale.y + detalScale, Image.transform.localScale.z);
            color.a += DELTA_TIME / time;
            Image.color = color;
            yield return new WaitForSeconds(DELTA_TIME);
        }
        Image.color = color;
    }
}
public class ClearImageEvent : ImageEvent
{
    public override IEnumerator Execute(EventObject eventObject = null)
    {
        Color color = Image.color;
        color.a = 0;
        Image.color = color;
        Image.sprite = null;
        return null;
    }
}

public class MoveImageEvent : ImageEvent
{
    [SerializeField] private Sprite sprite;
    [SerializeField] private Vector2 startPosition = new Vector2(0, 500);
    [SerializeField] private Vector2 endPosition = new Vector2(0, 0);
    [SerializeField] private float moveTime = 1f;
    [SerializeField] private Vector2 targetSize = new Vector2(100, 100);
    private const float DELTA_TIME = 0.02f;

    public override IEnumerator Execute(EventObject eventObject = null)
    {
        Image.sprite = sprite;
        Image.color = new Color(Image.color.r, Image.color.g, Image.color.b, 0);

        RectTransform rectTransform = Image.rectTransform;
        rectTransform.anchoredPosition = startPosition;
        rectTransform.sizeDelta = targetSize;

        float timer = 0f;
        Color color = Image.color;

        while (timer < moveTime)
        {
            timer += DELTA_TIME;
            float t = Mathf.Clamp01(timer / moveTime);

            rectTransform.anchoredPosition = Vector2.Lerp(startPosition, endPosition, t);

            color.a = t;
            Image.color = color;

            yield return new WaitForSeconds(DELTA_TIME);
        }

        rectTransform.anchoredPosition = endPosition;
        color.a = 1f;
        Image.color = color;
    }
}