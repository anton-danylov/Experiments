import { IPrimePalindrome, PrimePalindrome } from './prime-value-objects';
import { IPrimeChecker } from './prime-checkers';

export { IPrimePalindromesFinder, PrimePalindromesFinder };

interface IPrimePalindromesFinder {
    getPalindromes(from: number, to: number) : IPrimePalindrome[]
}

class PrimePalindromesFinder implements IPrimePalindromesFinder {
    private readonly checker:IPrimeChecker;

    constructor(primeChecker: IPrimeChecker) {
        this.checker = primeChecker;
    };

    getPalindromes(from: number, to: number) : IPrimePalindrome[] {
        let result: IPrimePalindrome[] = [];
    
    
        for (let first = from; first <= to; first++) {
            if (!this.checker.isPrime(first)) {
                continue;
            }
    
            for (let second = from; second <= to; second++) {
                if (!this.checker.isPrime(second)) {
                    continue;
                }
    
                if (this.isPalindrome(first * second)) {
                    result.push(new PrimePalindrome(first, second))
                }
            }
        }
    
        return result;
    }
    
    getDigit(num: number, index: number) {
        let order = Math.pow(10, index);
    
        return Math.floor((num / order) % 10);
    }
    
    isPalindrome(num: number): boolean {
        let digitsCount = Math.ceil(Math.log(num) / Math.LN10);
    
        for (let i = 0; i < digitsCount / 2; i++) {
            let digitStart = this.getDigit(num, i);
            let digitEnd = this.getDigit(num, digitsCount - i - 1);
    
            if (digitStart != digitEnd) {
                return false;
            }
        }
    
        return true;
    }
}