using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class logic : MonoBehaviour
{
    public Animator a;
    public GameObject impact;
    public AudioSource s;
    public AudioClip clip;

    Socket socket;
    EndPoint clientEnd;
    IPEndPoint ipEnd;
    string recvStr;
    byte[] recvData = new byte[1024];
    int recvLen;
    Thread connectThread;

    //В отдельном потоке прослушивается сокет
    IEnumerator SocketReceive()
    {
        while (true)
        {
            yield return null;
            if (socket.Available > 0)
            {
                recvData = new byte[1024];
                recvLen = socket.ReceiveFrom(recvData, ref clientEnd);
                recvStr = Encoding.ASCII.GetString(recvData, 0, recvLen);
                Shoot();
            }
        }
    }
    void SocketQuit()
    {
        if (connectThread != null)
        {
            connectThread.Interrupt();
            connectThread.Abort();
        }
        if (socket != null)
            socket.Close();
    }
    void OnApplicationQuit()
    {
        SocketQuit();
    }
    
    void Start()
    {
        //Создаём UDP сокет
        ipEnd = new IPEndPoint(IPAddress.Any, 8888);
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        socket.Bind(ipEnd);
        socket.Blocking = false;
        IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);
        clientEnd = sender;
        StartCoroutine(SocketReceive());
    }
    //метод стрельбы
    public void Shoot()
    {
        a.SetTrigger("Fire");
        s.PlayOneShot(clip);
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, 100))
        {
            Destroy(Instantiate(impact, hit.point, Quaternion.LookRotation(hit.normal)), 1);
            if (hit.transform.tag == "target")
                hit.transform.GetComponent<target>().take_bullet();
        }
    }
}

