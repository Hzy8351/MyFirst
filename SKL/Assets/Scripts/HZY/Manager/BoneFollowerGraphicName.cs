
using Spine.Unity;
using UnityEngine;

//��չToggleTest����Inspector������ʾ����
public class BoneFollowerGraphicName : MonoBehaviour
{
    public string path;
    public BoneFollowerGraphic boneFollwer;
    public void SetBoneName()
    {
        Debug.Log("boneFollwer.boneName:" + boneFollwer.boneName);
        //ִ�з���
        boneFollwer.boneName = path;
    }
}
