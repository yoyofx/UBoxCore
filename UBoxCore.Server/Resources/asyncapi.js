
//var nullFn = () => {};

function IllegalAPIException(name) {
  this.message = "No Such API [" + name + "]";
  this.name = 'IllegalAPIException';
}


/**浅拷贝工具方法**/
function clone(myObj) {
  if (typeof(myObj) != 'object' || myObj == null) return myObj;
  var newObj = new Object();
  for (var i in myObj) {
    newObj[i] = clone(myObj[i]);
  }
  return newObj;
}
/*代理实现类*/
function ProxyCopy(target, handle) {
  var targetCopy = clone(target);
  Object.keys(targetCopy).forEach(function(key) {
    Object.defineProperty(targetCopy, key, {
      get: function() {
        return handle.get && handle.get(target, key);
      },
      set: function(newVal) {
        handle.set && handle.set();
        target[key] = newVal;
      }
    })
  })

  Object.keys(target).forEach(function(key) {
    Object.defineProperty(targetCopy, key, {
      get: function() {
        return handle.get && handle.get(target, key);
      },
      set: function(newVal) {
        handle.set && handle.set();
        target[key] = newVal;
      }
    })
  })
  return targetCopy;
}

var ProxyFunc = ProxyCopy

try {
  if (Proxy) {
    ProxyFunc = Proxy
  }
} catch (ex) {
  ProxyFunc = ProxyCopy
}




