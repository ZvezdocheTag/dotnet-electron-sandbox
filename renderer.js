const { ipcRenderer } = require('electron')
const path = require('path')
var version = location.search.split('version=')[1]
var namespace = 'QuickStart.' + version.charAt(0).toUpperCase() + version.substr(1)
if (version === 'core') version = 'coreapp'

const baseNetAppPath = path.join(__dirname, '/src2/' + namespace + '/bin/Debug/net' + version + '2.0')

process.env.EDGE_USE_CORECLR = 1
if (version !== 'standard') { process.env.EDGE_APP_ROOT = baseNetAppPath }

var edge = require('electron-edge-js')

var baseDll = path.join(baseNetAppPath, namespace + '.dll')

var localTypeName = namespace + '.LocalMethods'
var externalTypeName = namespace + '.ExternalMethods'

var getAppDomainDirectory = edge.func({
  assemblyFile: baseDll,
  typeName: localTypeName,
  methodName: 'GetAppDomainDirectory'
})

// var getCurrentTime = edge.func({
//   assemblyFile: baseDll,
//   typeName: localTypeName,
//   methodName: 'GetCurrentTime'
// })

var useDynamicInput = edge.func({
  assemblyFile: baseDll,
  typeName: localTypeName,
  methodName: 'UseDynamicInput'
})

// var getPerson = edge.func({
//   assemblyFile: baseDll,
//   typeName: externalTypeName,
//   methodName: 'GetPersonInfo'
// })

var GetInput = edge.func({
  assemblyFile: baseDll,
  typeName: externalTypeName,
  methodName: 'GetInput'
})

// var helloWorld = edge.func(function () { /*
//   public async Task<object> Invoke(int command)
//   {
//       await Task.Run(() => {

//         return int command
//       }

//   }
// */ })
ipcRenderer.on('asynchronous-reply', (event, arg) => {
  document.getElementById('GetCurrentTime').innerHTML = new Date().toString()
  console.log(arg) // prints "pong"
})
GetInput('notepad', function (error, result) {
  // if (error) throw JSON.stringify(error);
  console.log(result)
  setTimeout(() => {
    ipcRenderer.send('asynchronous-message', 'ping')
  }, 1000)
  // document.getElementById('GetPersonInfo').innerHTML = result
})

window.onload = function () {
  document.getElementById('type_name').addEventListener('change', function (e) {
    console.log(e.target.value)
    useDynamicInput(e.target.value, function (error, result) {
      if (error) throw error
      document.getElementById('UseDynamicInput').innerHTML = result
    })
  })

  getAppDomainDirectory('', function (error, result) {
    if (error) throw error
    document.getElementById('GetAppDomainDirectory').innerHTML = result
  })

  // getCurrentTime('', function (error, result) {
  //   if (error) throw error
  //   document.getElementById('GetCurrentTime').innerHTML = result
  // })

  // document.getElementById('type_name').addEventListener('change', function (e) {
  //   GetInput(e.target.value, function (error, result) {
  //     // if (error) throw JSON.stringify(error);
  //     console.log(result)
  //     // document.getElementById('GetPersonInfo').innerHTML = result
  //   })
  // })

  // getPerson('', function (error, result) {
  //   // if (error) throw JSON.stringify(error);
  //   document.getElementById('GetPersonInfo').innerHTML = result
  // })
}
