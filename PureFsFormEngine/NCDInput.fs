namespace NCurGameEngine
module NCDInput =
  open SharpDX.DirectInput
  let input = new DirectInput()
  type StickErr =
    |NotInited
    |Disconnected

  let initByGUID guid (stick:Joystick option) =
    match stick with
      |Some j->j.Dispose()
      |None -> ()
    new Joystick(input, guid);

  type JoypadType =
    |Key
    |Joy
    |No

  let toJoy d =
    match d with
    |DeviceType.Joystick -> Joy
    |DeviceType.Gamepad  -> Joy
    |DeviceType.Keyboard -> Key
    |_ -> No

  let mutable mayJoyType = No
  let init() =
    let findDevices (dtype:DeviceType) = [for i in input.GetDevices(dtype, DeviceEnumerationFlags.AllDevices)->i]
    let stick = findDevices DeviceType.Joystick
    let pad = findDevices DeviceType.Gamepad
    let key = findDevices DeviceType.Keyboard
    let devs = stick @ pad @ key
    let dev = devs.Head
    mayJoyType <- toJoy dev.Type
    let joy = initByGUID dev.InstanceGuid None
    //joy.GetObjects()
    joy

  let lower = 34000
  let center = 30000
  let isXMin (state :JoystickState) =
    state.X < center
  let isXMax (state :JoystickState) =
    lower < state.X
  let isYMin (state :JoystickState) =
    state.Y < center
  let isYMax (state :JoystickState) =
    lower < state.Y

  let isRXMin (state :JoystickState) =
    state.Z < center
  let isRYMin (state :JoystickState) =
    state.RotationZ < center
  let isRXMax (state :JoystickState) =
    lower < state.Z
  let isRYMax (state :JoystickState) =
    lower < state.RotationZ

  let A(state :JoystickState) =
    state.Buttons.[2]
  let B(state :JoystickState) =
    state.Buttons.[1]
  //let up 0 right 9000 down 18000 left 27000

  let isLeftKey (state :JoystickState) =
    state.Buttons.[99]
  let isRightKey (state :JoystickState) =
    state.Buttons.[100]
  let isUpKey (state :JoystickState) =
    state.Buttons.[97]
  let isDownKey (state :JoystickState) =
    state.Buttons.[102]

  let isLeft state =
      match mayJoyType with
      |Joy -> isXMin state
      |Key -> isLeftKey state
      |No  -> false
  let isRight state =
      match mayJoyType with
      |Joy -> isXMax state
      |Key -> isRightKey state
      |No  -> false
  let isUp state =
      match mayJoyType with
      |Joy -> isYMin state
      |Key -> isUpKey state
      |No  -> false
  let isDown state =
      match mayJoyType with
      |Joy -> isYMax state
      |Key -> isDownKey state
      |No  -> false

  let update(stick:Joystick) =
    try 
      stick.Acquire()
      stick.Poll()
    // 抜けた
    with e -> ()// stick.Dispose()
    let state = stick.GetCurrentState()
    state

