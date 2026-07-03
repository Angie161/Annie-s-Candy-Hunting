using UnityEngine;

public class ButtonSkin : MonoBehaviour
{
    public GameObject selectedImage;
    public GameObject previewImage1;
    public GameObject previewImage2;

    public void SetSelected(bool value)
    {
        if (selectedImage != null)
            selectedImage.SetActive(value);

        if (previewImage1 != null)
            previewImage1.SetActive(value);

        if (previewImage2 != null)
            previewImage2.SetActive(value);
    }
}