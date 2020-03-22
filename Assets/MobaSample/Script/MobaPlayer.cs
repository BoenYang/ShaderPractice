using System.Collections;
using System.Collections.Generic;
using NetworkProtocal;
using UnityEngine;

public class MobaPlayer : MonoBehaviour
{
    public float MoveSpeed = 5;

    public float RotateSpeed = 5;

    [System.NonSerialized]
    public float VertivalInput;

    [System.NonSerialized]
    public float HorizontalInput;

    private Vector3 m_MoveDir = Vector3.zero;

    public void Update()
    {
        Vector3 position = transform.position;

        m_MoveDir.x = HorizontalInput;
        m_MoveDir.z = VertivalInput;

        transform.position = position + m_MoveDir * MoveSpeed * Time.deltaTime;

        if (!Mathf.Approximately(Mathf.Abs(m_MoveDir.x), 0) || !Mathf.Approximately(Mathf.Abs(m_MoveDir.z), 0))
        {
            Quaternion lookAt = Quaternion.LookRotation(m_MoveDir);
            if (!lookAt.Equals(Quaternion.identity)) {
                Quaternion rotation = transform.rotation;
                transform.rotation = Quaternion.Lerp(rotation, lookAt, RotateSpeed * Time.deltaTime);
            }
        }
    }

    public void SyncPos()
    {
        PlayerGameInfo gameInfo = new PlayerGameInfo();
        gameInfo.MoveX = (int)m_MoveDir.x * 100;
        gameInfo.MoveZ = (int)m_MoveDir.z * 100;
        MobaClient.Instance.Send(GameCmd.SyncRequest,gameInfo);
    }

}
