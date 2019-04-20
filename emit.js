const EventEmitter = require('events')

class MyEmitter extends EventEmitter {}

let m = 0
// IN this example async method happens two times after invoke
const myEmitter = new MyEmitter()

// Only do this once so we don't loop forever
myEmitter.once('newListener', (event, listener) => {
  if (event === 'event') {
    // Insert a new listener in front
    myEmitter.on('event', () => {
      console.log('B')
    })
  }
})

myEmitter.on('event', function (a, b) {
  setImmediate(() => {
    console.log('this happens asynchronously')
  })
  console.log(a, b, this, this === myEmitter
  )
  console.log('an event occurred!')
  console.log(++m)
})

myEmitter.on('error', (err) => {
  console.error(err, 'whoops! there was an error')
})

myEmitter.emit('event', 'a', 'b')

// myEmitter.emit('error', new Error('whoops!'))

myEmitter.emit('event')

const myEE = new EventEmitter()
myEE.on('foo', () => {})
myEE.on('bar', () => {})

const sym = Symbol('symbol')
myEE.on(sym, () => {})

console.log(myEE.eventNames())

const ee = new EventEmitter()

function pong () {
  console.log('pong')
}

ee.on('ping', pong)
ee.once('ping', pong)
ee.removeListener('ping', pong)

ee.emit('ping')
ee.emit('ping')
