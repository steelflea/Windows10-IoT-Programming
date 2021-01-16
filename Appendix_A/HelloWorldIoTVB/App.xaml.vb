''' <summary>
''' 기본 애플리케이션 클래스를 보완하는 애플리케이션별 동작을 제공합니다.
''' </summary>
NotInheritable Class App
    Inherits Application

    ''' <summary>
    ''' 최종 사용자가 애플리케이션을 정상적으로 시작할 때 호출됩니다. 다른 진입점은
    ''' 특정 파일을 열거나, 검색 결과를 표시하는 등 애플리케이션을 시작할 때
    ''' 사용됩니다.
    ''' </summary>
    ''' <param name="e">시작 요청 및 프로세스에 대한 정보입니다.</param>
    Protected Overrides Sub OnLaunched(e As Windows.ApplicationModel.Activation.LaunchActivatedEventArgs)
        Dim rootFrame As Frame = TryCast(Window.Current.Content, Frame)

        ' 창에 콘텐츠가 이미 있는 경우 앱 초기화를 반복하지 말고,
        ' 창이 활성화되어 있는지 확인하십시오.

        If rootFrame Is Nothing Then
            ' 탐색 컨텍스트로 사용할 프레임을 만들고 첫 페이지로 이동합니다.
            rootFrame = New Frame()

            AddHandler rootFrame.NavigationFailed, AddressOf OnNavigationFailed

            If e.PreviousExecutionState = ApplicationExecutionState.Terminated Then
                ' TODO: 이전에 일시 중지된 애플리케이션에서 상태를 로드합니다.
            End If
            ' 현재 창에 프레임 넣기
            Window.Current.Content = rootFrame
        End If

        If e.PrelaunchActivated = False Then
            If rootFrame.Content Is Nothing Then
                ' 탐색 스택이 복원되지 않으면 첫 번째 페이지로 돌아가고
                ' 필요한 정보를 탐색 매개 변수로 전달하여 새 페이지를
                ' 구성합니다.
                rootFrame.Navigate(GetType(MainPage), e.Arguments)
            End If

            ' 현재 창이 활성 창인지 확인
            Window.Current.Activate()
        End If
    End Sub

    ''' <summary>
    ''' 특정 페이지 탐색에 실패한 경우 호출됨
    ''' </summary>
    ''' <param name="sender">탐색에 실패한 프레임</param>
    ''' <param name="e">탐색 실패에 대한 정보</param>
    Private Sub OnNavigationFailed(sender As Object, e As NavigationFailedEventArgs)
        Throw New Exception("Failed to load Page " + e.SourcePageType.FullName)
    End Sub

    ''' <summary>
    ''' 애플리케이션 실행이 일시 중단된 경우 호출됩니다.  애플리케이션이 종료될지
    ''' 또는 메모리 콘텐츠를 변경하지 않고 다시 시작할지 여부를 결정하지 않은 채
    ''' 응용 프로그램 상태가 저장됩니다.
    ''' </summary>
    ''' <param name="sender">일시 중단 요청의 소스입니다.</param>
    ''' <param name="e">일시 중단 요청에 대한 세부 정보입니다.</param>
    Private Sub OnSuspending(sender As Object, e As SuspendingEventArgs) Handles Me.Suspending
        Dim deferral As SuspendingDeferral = e.SuspendingOperation.GetDeferral()
        ' TODO: 애플리케이션 상태를 저장하고 백그라운드 작업을 모두 중지합니다.
        deferral.Complete()
    End Sub

End Class
