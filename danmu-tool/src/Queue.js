export default class Queue {
	constructor(items) {
		this.items = items || []
	}

	// 入列
	enqueue(ele) {
		this.items.push(ele)
	}

	// 出列
	dequeue() {
		return this.items.shift()
	}

	clear() {
		this.items = []
	}
	get size() {
		return this.items.length
	}

	get isEmpty() {
		return !this.items.length
	}

	print() {
		console.log(this.items.toString())
	}
}
