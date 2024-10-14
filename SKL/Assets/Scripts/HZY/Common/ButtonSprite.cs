using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ButtonSprite : MonoBehaviour
{
	public enum BTN_TYPE
	{
		DEFAULT,
		NORMAL,
	};

	//public string name = "ButtonSprite";
	public BTN_TYPE btnType = BTN_TYPE.DEFAULT;
	public UnityEvent addEvent;

	private Sprite defauleSprite;
	public Sprite clickedSprite;

	private SpriteRenderer render;
	private Color mColor;

	void Start()
	{
		render = GetComponent<SpriteRenderer>();
		defauleSprite = render.sprite;
		mColor = render.color;
	}

	void OnMouseDown()
	{
		if (btnType == BTN_TYPE.DEFAULT)
		{
			render.color = Color.gray;
		}
		else if (btnType == BTN_TYPE.NORMAL)
		{
			render.sprite = clickedSprite;
		}
	}

	void OnMouseUp()
	{
		if (btnType == BTN_TYPE.DEFAULT)
		{
			render.color = mColor;
		}
		else if (btnType == BTN_TYPE.NORMAL)
		{
			render.sprite = defauleSprite;
		}
		MyEvent();
	}

	public void MyEvent()
	{
		addEvent.Invoke();
	}


}
