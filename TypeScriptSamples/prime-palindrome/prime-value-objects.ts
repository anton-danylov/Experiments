export { IPrimePalindrome, PrimePalindrome };

interface IPrimePalindrome {
    readonly firstPrime: number;
    readonly secondPrime: number;

    palindrome() : number;
    toString(): string;
}

class PrimePalindrome implements IPrimePalindrome {
    readonly firstPrime: number;
    readonly secondPrime: number;

    constructor(first: number, second: number) {
        this.firstPrime = first;
        this.secondPrime = second;
    }

    palindrome() { 
        return this.firstPrime * this.secondPrime;
    }

    toString(): string {
        return `${this.firstPrime} * ${this.secondPrime} = ${this.palindrome()}`;
    }
}