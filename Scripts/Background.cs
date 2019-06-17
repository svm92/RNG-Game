using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Background : MonoBehaviour {

    public Sprite BGTriangle;
    public Sprite BGSquare;
    public Sprite BGHexagon;
    public Sprite BGDodecagon;

    public bool isTitleScreen = false;

    enum TileType { TRIANGLE, SQUARE, HEXAGON, DODECAGON };
    TileType tileType;
    enum MovementType { SLIDE, ROTATE };
    MovementType movementType;
    enum ShaderType { RGB_CYCLE, CMY_CYCLE, LINEAR_GRADIENT, BILINEAR_GRADIENT, CIRCLE_GRADIENT, DOUBLE_LINEAR_GRADIENT,
                    MOVING_CIRCLES};
    ShaderType shaderType;

    float tileSizeX;
    float tileSizeY;

    float scrollSpeed;
    float scrollMaxLengthX;
    float scrollMaxLengthY;
    Vector2 horizontalDirection;
    Vector2 verticalDirection;

    float rotationSpeed;
    float rotationTime;
    float lowerScale;
    float upperScale;
    Vector3 rotationDirection;
    
    Vector2 originalPosition;
    RectTransform rectTr;
    Material material;
    Image image;

    private void Start()
    {
        originalPosition = transform.position;
        rectTr = GetComponent<RectTransform>();
        image = GetComponent<Image>();
        material = image.material;

        if (BattleManager.isTutorial)
        {
            image.sprite = BGSquare;
            image.type = Image.Type.Simple;
            image.color = Color.black;
            image.material = null;
            return;
        }

        tileType = (!isTitleScreen) ? chooseRandomTileType() : TileType.SQUARE;
        switch (tileType)
        {
            case TileType.TRIANGLE:
                image.sprite = BGTriangle;
                tileSizeX = 391;
                tileSizeY = 341;
                break;
            case TileType.SQUARE:
                image.sprite = BGSquare;
                transform.localScale = Random.Range(1.75f, 3f) * Vector2.one; 
                tileSizeX = tileSizeY = 77;
                if (isTitleScreen) image.type = Image.Type.Simple;
                break;
            case TileType.HEXAGON:
                image.sprite = BGHexagon;
                transform.localScale = Random.Range(.5f, 1.1f) * Vector2.one;
                tileSizeX = 551;
                tileSizeY = 639;
                break;
            case TileType.DODECAGON:
                image.sprite = BGDodecagon;
                tileSizeX = 463;
                tileSizeY = 403;
                break;
        }

        movementType = (!isTitleScreen) ? chooseRandomMovementType() : MovementType.SLIDE;
        switch (movementType)
        {
            case MovementType.SLIDE:
                // Tiles have a set width and height, so restart cycles after those lengths
                // The length must be measured in absolute units, so transform it from canvas units first
                scrollMaxLengthX = GetComponent<RectTransform>().TransformPoint(tileSizeX, 0, 0).x
                    - GetComponent<RectTransform>().TransformPoint(0, 0, 0).x;
                scrollMaxLengthY = GetComponent<RectTransform>().TransformPoint(0, tileSizeY, 0).y
                    - GetComponent<RectTransform>().TransformPoint(0, 0, 0).y;

                // Choose random speed/direction
                scrollSpeed = (!isTitleScreen) ? Random.Range(0.5f, 1.75f) : 0f;
                horizontalDirection = (Random.Range(0, 2) == 0) ? Vector2.right : Vector2.left;
                verticalDirection = (Random.Range(0, 2) == 0) ? Vector2.up : Vector2.down;
                break;
            case MovementType.ROTATE:
                rotationSpeed = Random.Range(0.035f, 0.045f);
                rotationTime = Random.Range(14.5f, 21.5f);
                lowerScale = Random.Range(0.75f, 1.1f);
                upperScale = Random.Range(0.55f, 1.25f);
                rotationDirection = (Random.Range(0, 2) == 0) ? Vector3.forward : Vector3.back;
                break;
        }

        shaderType = (!isTitleScreen) ? chooseRandomShader() : ShaderType.BILINEAR_GRADIENT;
        switch (shaderType)
        {
            case ShaderType.RGB_CYCLE:
                material.shader = Shader.Find("Custom/RGBCycleShader");
                material.SetFloat("_Duration", Random.Range(4f, 10f));
                material.SetFloat("_Overlap", Random.Range(.15f, .4f));
                int invertValue = (Random.Range(0, 2) == 0) ? 0 : 1;
                material.SetFloat("_Invert", invertValue);
                break;
            case ShaderType.CMY_CYCLE:
                material.shader = Shader.Find("Custom/CMYCycleShader");
                material.SetFloat("_Duration", Random.Range(6.5f, 9f));
                invertValue = (Random.Range(0, 2) == 0) ? 0 : 1;
                material.SetFloat("_Invert", invertValue);
                break;
            case ShaderType.LINEAR_GRADIENT:
                material.shader = Shader.Find("Custom/LinearGradientShader");
                material.SetFloat("_Duration", Random.Range(4f, 7f));
                material.SetFloat("_ROffset", Mathf.Floor(Random.Range(0f, 2f)));
                material.SetFloat("_GOffset", Mathf.Floor(Random.Range(2f, 4f)));
                material.SetFloat("_BOffset", Mathf.Floor(Random.Range(4f, 6f)));
                material.SetFloat("_Direction", Random.Range(0, 2));
                break;
            case ShaderType.BILINEAR_GRADIENT:
                material.shader = Shader.Find("Custom/BilinearGradientShader");
                material.SetFloat("_Duration",Random.Range(4f, 7f));
                material.SetFloat("_ROffset", Mathf.Floor(Random.Range(0f, 2f)));
                material.SetFloat("_GOffset", Mathf.Floor(Random.Range(2f, 4f)));
                material.SetFloat("_BOffset", Mathf.Floor(Random.Range(4f, 6f)));
                material.SetFloat("_Phase", Random.Range(0f, 60f));
                break;
            case ShaderType.CIRCLE_GRADIENT:
                material.shader = Shader.Find("Custom/CircleGradientShader");
                material.SetFloat("_Duration", Random.Range(4f, 12f));
                material.SetFloat("_CenterX", .5f);
                material.SetFloat("_CenterY", .5f);
                break;
            case ShaderType.DOUBLE_LINEAR_GRADIENT:
                material.shader = Shader.Find("Custom/DoubleLinearGradientShader");
                material.SetFloat("_Duration1", Random.Range(3f, 6f));
                material.SetFloat("_Duration2", Random.Range(5f, 9f));
                material.SetFloat("_ROffset", Mathf.Floor(Random.Range(0f, 2f)));
                material.SetFloat("_GOffset", Mathf.Floor(Random.Range(2f, 4f)));
                material.SetFloat("_BOffset", Mathf.Floor(Random.Range(4f, 6f)));
                material.SetFloat("_Direction", Random.Range(0, 2));
                break;
            case ShaderType.MOVING_CIRCLES:
                material.shader = Shader.Find("Custom/MovingCirclesShader");
                string[] colors = new string[] { "R", "G", "B", "C", "M", "Y" };
                // Assign random starting positions/scales for each color
                foreach (string color in colors)
                {
                    material.SetVector("_Center" + color, new Vector2(Random.Range(0f, 1f), Random.Range(0f, 1f)));
                    material.SetFloat("_Radius" + color, Random.Range(0.15f, 0.35f));
                    StartCoroutine(moveCircle(color));
                    StartCoroutine(scaleCircle(color));
                }
                break;
        }
    }

    private void OnEnable()
    {
        switch (shaderType)
        {
            case ShaderType.MOVING_CIRCLES:
                string[] colors = new string[] { "R", "G", "B", "C", "M", "Y" };
                foreach (string color in colors) // Make circles start moving/scaling again
                {
                    StartCoroutine(moveCircle(color));
                    StartCoroutine(scaleCircle(color));
                }
                break;
        }
    }

    private void Update()
    {
        if (BattleManager.isTutorial) return;
        switch (movementType)
        {
            case MovementType.SLIDE:
                Vector2 horizontalDisplacement = ((Time.time * scrollSpeed) % scrollMaxLengthX) * horizontalDirection;
                Vector2 verticalDisplacement = ((Time.time * scrollSpeed) % scrollMaxLengthY) * verticalDirection;
                transform.position = originalPosition + horizontalDisplacement + verticalDisplacement;
                break;
            case MovementType.ROTATE:
                transform.Rotate(rotationDirection, rotationSpeed);
                float newScale = lowerScale + Mathf.Abs(Mathf.Sin(Time.time * (Mathf.PI / 2) / rotationTime) * upperScale);
                transform.localScale = Vector3.one * newScale;
                break;
        }

        switch (shaderType)
        {
            case ShaderType.LINEAR_GRADIENT: // Randomly choose new offsets (slightly increasing or decreasing hue)
            case ShaderType.BILINEAR_GRADIENT:
                float chance = 0.01f;
                float rnd = Random.Range(0f, 1f);
                if (rnd < chance)
                {
                    float currentValue = material.GetFloat("_ROffset");
                    float newValue = Mathf.Clamp(currentValue + Random.Range(-0.01f, 0.01f), 0f, 2f);
                    material.SetFloat("_ROffset", newValue);
                }

                rnd = Random.Range(0f, 1f);
                if (rnd < chance)
                {
                    float currentValue = material.GetFloat("_GOffset");
                    float newValue = Mathf.Clamp(currentValue + Random.Range(-0.01f, 0.01f), 2f, 4f);
                    material.SetFloat("_GOffset", newValue);
                }

                rnd = Random.Range(0f, 1f);
                if (rnd < chance)
                {
                    float currentValue = material.GetFloat("_BOffset");
                    float newValue = Mathf.Clamp(currentValue + Random.Range(-0.01f, 0.01f), 4f, 6f);
                    material.SetFloat("_BOffset", newValue);
                }
                break;
        }
    }

    IEnumerator moveCircle(string color)
    {
        Vector2 originalPosition = material.GetVector("_Center" + color);
        Vector2 destination = new Vector2(Random.Range(0f, 1f), Random.Range(0f, 1f));
        float speed = 0.45f;
        float travelDistance = Vector2.Distance(originalPosition, destination);
        float travelDuration = travelDistance / speed;
        float timer = 0;

        while (timer <= travelDuration)
        {
            Vector2 nextPosition = Vector2.Lerp(originalPosition, destination, timer / travelDuration);
            material.SetVector("_Center" + color, nextPosition);
            timer += Time.deltaTime;
            yield return null;
        }
        
        StartCoroutine(moveCircle(color));
    }

    IEnumerator scaleCircle(string color)
    {
        float originalScale = material.GetFloat("_Radius" + color);
        float finalScale = Random.Range(0.15f, 0.35f);
        float transitionTime = 3f;
        float timer = 0;

        while (timer <= transitionTime)
        {
            float nextScale = Mathf.Lerp(originalScale, finalScale, timer / transitionTime);
            material.SetFloat("_Radius" + color, nextScale);
            timer += Time.deltaTime;
            yield return null;
        }

        StartCoroutine(scaleCircle(color));
    }

    TileType chooseRandomTileType()
    {
        int rnd = Random.Range(0, System.Enum.GetValues(typeof(TileType)).Length);
        return (TileType)rnd;
    }

    MovementType chooseRandomMovementType()
    {
        int rnd = Random.Range(0, System.Enum.GetValues(typeof(MovementType)).Length);
        return (MovementType)rnd;
    }

    ShaderType chooseRandomShader()
    {
        int rnd = Random.Range(0, System.Enum.GetValues(typeof(ShaderType)).Length);
        return (ShaderType)rnd;
    }
}
