
using Spine.Unity;
using UnityEngine;

//��չToggleTest����Inspector������ʾ����
public class BoneFollowerName : MonoBehaviour
{
    public string path;
    public BoneFollower boneFollwer;
    public void SetBoneName()
    {
        Debug.Log("boneFollwer.boneName:" + boneFollwer.boneName);
        //ִ�з���
        boneFollwer.boneName = path;
    }
}

