using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayIconTrigger : MonoBehaviour
{
    [SerializeField]
    private ButtonType buttonType;

    private Transform icon;
    [SerializeField]
    private GameObject buttonIcon;
    [SerializeField]
    private Vector3 offset = new Vector3(0f, 2f);
    private bool showIcon = true;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.tag == "Player" && icon == null)
        {
            icon = Instantiate(buttonIcon, transform.position + offset, Quaternion.identity).transform;
            icon.GetComponent<ButtonIcon>().Initialize(buttonType);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.transform.tag == "Player")
            DestroyIcon();
    }

    private void Update()
    {
        if (icon != null)
        {
            if (transform.position.y > 0f)
                icon.position = transform.position + offset;
            else
                icon.position = transform.position - offset;

            icon.GetComponent<SpriteRenderer>().enabled = showIcon;
        }

    }

    public void DestroyIcon()
    {
        if (icon != null)
            Destroy(icon.gameObject);
    }

    // TODO: Add this to the box
    public void SetShowIcon(bool show)
    {
        showIcon = show;
    }
}
