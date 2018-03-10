# UnityCommandController

「UnityCommandController」は  
コマンドを制御してイベントスクリプトなどを実装できる簡易ライブラリです  

# 開発環境

- Unity 2017.3.0f3

# 導入方法

1. 下記のページにアクセスして「UnityCommandController.unitypackage」をダウンロードします  
https://github.com/baba-s/unity-command-controller/blob/master/UnityCommandController.unitypackage?raw=true
2. ダウンロードした「UnityCommandController.unitypackage」を Unity プロジェクトにインポートします  

# 使用例

```cs
using System;
using UnityCommandController;
using UnityEngine;

public class Example : MonoBehaviour
{
    private CommandController m_controller;

    private void Awake()
    {
        // 使用するコマンドの型リスト
        var commandTypes = new []
        {
            typeof( LogCommand ),
            typeof( CreateCommand ),
            typeof( SetPositionCommand ),
            typeof( MoveCommand ),
            typeof( JumpCommand ),
            typeof( WaitCommand ),
            typeof( ClickCommand ),
        };

        // コマンドを制御するインスタンスの作成
        m_controller = new CommandController( commandTypes );

        // コマンドのリストの作成
        var commands = new[]
        {
            "LogCommand|ピカチュウ",
            "LogCommand|ライチュウ",
            "CreateCommand|cube|0|1|2",
            "SetPositionCommand|1|2|3",
            "MoveCommand|-1|0|1|1",
            "JumpCommand|6",
            "LogCommand|ここは無視されます",
            "LogCommand|ここにジャンプします",
            "WaitCommand|1",
            "ClickCommand",
        };

        // コマンドで制御するオブジェクトやパラメータ
        GameObject cube = null;
        var startPos = Vector3.zero;
        var endPos = Vector3.zero;

        // コマンドのイベントを設定
        CreateCommand.OnCreate += go => cube = go;
        SetPositionCommand.OnSetPosition += pos => cube.transform.localPosition = pos;
        MoveCommand.OnMoveStart += pos =>
        {
            startPos = cube.transform.localPosition;
            endPos = pos;
        };
        MoveCommand.OnMove += amount =>
        {
            cube.transform.localPosition = Vector3.Lerp( startPos, endPos, amount );
        };
        MoveCommand.OnMoveEnd += () => cube.transform.localPosition = endPos;
        JumpCommand.OnJump += index => m_controller.JumpToIndex( index );

        // すべてのコマンドが終了した時に呼び出される
        m_controller.OnEnd += () => print( "終了" );

        // コマンドの開始
        m_controller.Start( commands );
    }

    private void Update()
    {
        // コマンドの更新
        m_controller.Update();
    }
}

public class LogCommand : CommandBase
{
    private string m_message;

    public LogCommand( CommandArugments args )
    {
        m_message = args[ 1 ];
    }

    protected override void DoStart()
    {
        Debug.Log( m_message );
    }
}

public class CreateCommand : CommandBase
{
    private string m_name;
    private Vector3 m_pos;

    public static event Action<GameObject> OnCreate = delegate { };

    public CreateCommand( CommandArugments args )
    {
        m_name = args[ 1 ];
        m_pos = new Vector3
        (
            args.ToFloat( 2 ),
            args.ToFloat( 3 ),
            args.ToFloat( 4 )
        );
    }

    protected override void DoStart()
    {
        var go = GameObject.CreatePrimitive( PrimitiveType.Cube );
        go.name = m_name;
        go.transform.localPosition = m_pos;
        OnCreate( go );
        Debug.Log( "オブジェクト作成完了" );
    }
}

public class SetPositionCommand : CommandBase
{
    private Vector3 m_pos;

    public static event Action<Vector3> OnSetPosition = delegate { };

    public SetPositionCommand( CommandArugments args )
    {
        m_pos = new Vector3
        (
            args.ToFloat( 1 ),
            args.ToFloat( 2 ),
            args.ToFloat( 3 )
        );
    }

    protected override void DoStart()
    {
        OnSetPosition( m_pos );
        Debug.Log( "オブジェクト位置設定完了" );
    }
}

public class MoveCommand : CommandBase
{
    private Vector3 m_pos;
    private float m_duration;
    private float m_elapsedTime;

    public override bool IsEnd { get { return m_duration <= m_elapsedTime; } }

    public static event Action<Vector3> OnMoveStart = delegate { };
    public static event Action<float> OnMove = delegate { };
    public static event Action OnMoveEnd = delegate { };

    public MoveCommand( CommandArugments args )
    {
        m_pos = new Vector3
        (
            args.ToFloat( 1 ),
            args.ToFloat( 2 ),
            args.ToFloat( 3 )
        );
        m_duration = args.ToFloat( 4 );
    }

    protected override void DoStart()
    {
        OnMoveStart( m_pos );
        Debug.Log( "オブジェクト移動開始" );
    }

    protected override void DoUpdate()
    {
        m_elapsedTime += Time.deltaTime;
        OnMove( m_elapsedTime / m_duration );
    }

    protected override void DoDispose()
    {
        OnMoveEnd();
        Debug.Log( "オブジェクト移動終了" );
    }
}

public class JumpCommand : CommandBase
{
    private int m_index;

    public static event Action<int> OnJump = delegate { };

    public JumpCommand( CommandArugments args )
    {
        m_index = args.ToInt( 1 );
    }

    protected override void DoStart()
    {
        OnJump( m_index );
    }
}

public class WaitCommand : CommandBase
{
    private float m_time;
    private float m_elapsedTime;

    public override bool IsEnd { get { return m_time <= m_elapsedTime; } }

    public WaitCommand( CommandArugments args )
    {
        m_time = args.ToFloat( 1 );
    }

    protected override void DoStart()
    {
        Debug.Log( "待機開始" );
    }

    protected override void DoUpdate()
    {
        m_elapsedTime += Time.deltaTime;
    }

    protected override void DoDispose()
    {
        Debug.Log( "待機終了" );
    }
}

public class ClickCommand : CommandBase
{
    public override bool IsEnd { get { return Input.GetMouseButtonDown( 0 ); } }

    public ClickCommand( CommandArugments args ) { }

    protected override void DoStart()
    {
        Debug.Log( "入力待ち" );
    }

    protected override void DoDispose()
    {
        Debug.Log( "入力確認" );
    }
}
```