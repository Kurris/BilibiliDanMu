export default class Queue<T> {
    private _items: T[] = []

    // 入列
    enqueue(ele: T) {
        this._items.push(ele)
    }

    // 出列
    dequeue(): T | undefined {
        return this._items.shift()
    }

    clear() {
        this._items.splice(0, this.size)
    }

    get size() {
        return this._items.length
    }

    get isEmpty() {
        return !this._items.length
    }

    print() {
        console.log(this._items.toString())
    }
}
