export {IPrimeChecker, PrimeChecker, CachedPrimeChecker, MappedPrimeChecker};

interface IPrimeChecker {
    isPrime(num: number): boolean;
}

class PrimeChecker implements IPrimeChecker {
    isPrime(num: number): boolean {
        if (num <= 1) {
            return false;
        }
        else if (num <= 3) {
            return true;
        }
        else if (num % 2 == 0 || num % 3 == 0) {
            return false;
        }
        for (let i = 5; i * i <= num; i = i + 6) {
            if (num % i == 0 || num % (i + 2) == 0) {
                return false;
            }
        }
    
        return true;
    }
}

class CachedPrimeChecker implements IPrimeChecker {
    private readonly checkedPrimes: number[] = [];
    private readonly checkedNonPrimes: number[] = [];

    private readonly checker:IPrimeChecker = new PrimeChecker();

    isPrime(num: number): boolean {
        if (this.checkedPrimes.indexOf(num) != -1) {
            return true;
        }
        else if (this.checkedNonPrimes.indexOf(num) != -1) {
            return false;
        }
        else if (this.checker.isPrime(num)) {
            this.checkedPrimes.push(num);
            return true;
        }
        
        this.checkedNonPrimes.push(num);
        return false;
    }
}

class MappedPrimeChecker implements IPrimeChecker {
    private readonly checkedPrimes: boolean[] = [];
    private readonly checker:IPrimeChecker = new PrimeChecker();

    constructor(count?:number){
        if (count != undefined){
            this.checkedPrimes = new Array<boolean>(count);
        }
    }

    isPrime(num: number): boolean {
        if (this.checkedPrimes[num] != undefined) {
            return this.checkedPrimes[num];
        }

        let result = this.checker.isPrime(num);
        this.checkedPrimes[num] = result;

        return result;
    }
}
