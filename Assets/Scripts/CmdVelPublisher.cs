using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using System.Collections;
// 各種ROSトピック形式
using TwistMsg = RosMessageTypes.Geometry.TwistMsg;

/// <summary>
/// CmdVel（TwistMsg）を送信するためのクラス
/// UIから各関数を呼び出して使うことを想定
/// </summary>
public class CmdVelPublisher : MonoBehaviour
{
    // Variables required for ROS communication
    // 送信するROSのトピック名
    [SerializeField] string topicName = "cmd_vel";

    // 送信する速度指令の基準となる値（各関数でこの速度に所定の倍率をかける）
    [SerializeField] float linearVel;

    // 送信する角速度指令の基準となる値（各関数でこの角速度に所定の倍率をかける）
    [SerializeField] float angularVel;

    // ROS Connector
    private ROSConnection ros;

    private TwistMsg cmdVelMessage = new TwistMsg();

    // ボタンが押されているかどうかを保持する変数
    private bool isButtonPressed = false;

    // コルーチンのハンドル
    private Coroutine publishCoroutine;

    /// <summary>
    /// 初期化用のイベント関数
    /// https://docs.unity3d.com/ja/2020.3/Manual/ExecutionOrder.html
    /// </summary>
    void Start()
    {
        // Get ROS connection static instance
        ros = ROSConnection.GetOrCreateInstance();
        ros.RegisterPublisher<TwistMsg>(topicName);
        SetStopVel();
        Publish();
    }

    /// <summary>
    /// ボタンが押されたときに呼び出される関数
    /// </summary>
    public void OnButtonPress()
    {
        isButtonPressed = true;
        if (publishCoroutine == null)
        {
            publishCoroutine = StartCoroutine(PublishAtFixedRate(0.05f)); // 20Hz = 0.05秒間隔
        }
    }

    /// <summary>
    /// ボタンが離されたときに呼び出される関数
    /// </summary>
    public void OnButtonRelease()
    {
        isButtonPressed = false;
        if (publishCoroutine != null)
        {
            StopCoroutine(publishCoroutine);
            publishCoroutine = null;
        }
        SetStopVel();
        Publish();
    }

    /// <summary>
    /// 前進方向の速度指令を設定
    /// </summary>
    /// <param name="ratio">指令する速度の倍率</param>
    public void SetForwardVel(float ratio = 1.0f)
    {
        cmdVelMessage.linear.x = linearVel * ratio;
    }

    /// <summary>
    /// 後進方向の速度指令を設定
    /// </summary>
    /// <param name="ratio">指令する速度の倍率</param>
    public void SetBackwardVel(float ratio = 1.0f)
    {
        cmdVelMessage.linear.x = -linearVel * ratio;
    }

    /// <summary>
    /// 右旋回の角速度指令を設定
    /// </summary>
    /// <param name="ratio">指令する角速度の倍率</param>
    public void SetRightTurnVel(float ratio = 1.0f)
    {
        cmdVelMessage.angular.z = -angularVel * ratio;
    }

    /// <summary>
    /// 左旋回の角速度指令を設定
    /// </summary>
    /// <param name="ratio">指令する角速度の倍率</param>
    public void SetLeftTurnVel(float ratio = 1.0f)
    {
        cmdVelMessage.angular.z = angularVel * ratio;
    }

    /// <summary>
    /// 停止の速度指令、角速度指令を設定
    /// </summary>
    public void SetStopVel()
    {
        cmdVelMessage.linear.x = 0;
        cmdVelMessage.angular.z = 0;
    }

    /// <summary>
    /// 速度指令、角速度指令を送信
    /// </summary>
    public void Publish()
    {
        // ROSのros_tcp_endpointのdefault_server_endpoint.pyにメッセージを送信
        ros.Publish(topicName, cmdVelMessage);
    }

    /// <summary>
    /// 一定間隔でPublishを呼び出すコルーチン
    /// </summary>
    /// <param name="interval">呼び出し間隔（秒）</param>
    /// <returns></returns>
    private IEnumerator PublishAtFixedRate(float interval)
    {
        while (isButtonPressed)
        {
            Publish();
            yield return new WaitForSeconds(interval);
        }
    }
}
