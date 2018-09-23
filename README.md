# Invisionware.Core
Core components for the Invisionware framework

# Extensions
The following are the details o nthe extensions provided in this library

## Enumerable
The follow extensions are supported on an IEnumerable and IEnumerable<T> object

```
var lst = new List<string>() { "a", "b", "c", "d", "e", "f" }
```

### ChunkBy
Break up an existing array into chucnks of a certain size

```
var chunkResult = lst.ChunkBy(2); // returns 2 arrays with 3 items each
```

### TakeLast
Take the last N number of elements from an array

```
var last lst.TakLast(2); // returns a kist with "e" and "f"
```

### AnySafe
A "null safe" version of Any().  Returns 0 if the object is null

```
IList<string> invalidList = null;
var anySafe = invalidList.AnySafe(); // Returns false
```

## Enum

### GetAttributeOfType

### GetEnumValue

### TryConvert

## List

### RemoveRange

## String

### IsNullOrEmpty
Extends the string object to provide a "method" for checking if the string is null or empty.  Really not sure why this isn't part of the framework
```
string s = null;

Console.WriteLine(s.IsNullOrEmpty());  // Output: true
```

## Type

### GetAttributeValue

### GetAttributeOfType

### AttributeExists