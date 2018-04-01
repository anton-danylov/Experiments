import { IPrimeChecker, PrimeChecker, CachedPrimeChecker, MappedPrimeChecker } from './prime-checkers';
import { IPrimePalindrome, PrimePalindrome } from './prime-value-objects';
import { PrimePalindromesFinder } from './prime-palindromes-finder';


export function printPalindrome() {
    let contentNode = document.getElementById("content");
    
    
    console.log("Calculating...");

    let timerStart = performance.now();


    const promise = new Promise((resolve) => {    
        contentNode.appendChild(document.createTextNode("Waiting...."));

        setTimeout(() => {
            let checker = new MappedPrimeChecker(100000);
            let finder = new PrimePalindromesFinder(checker);
    
            let palindromes: IPrimePalindrome[] = finder.getPalindromes(10000, 99999);
            palindromes.sort((a, b) => b.palindrome() - a.palindrome());
            resolve(palindromes);
        }, 10);
    });
    
    promise.then((palindromes: IPrimePalindrome[]) => {
        while (contentNode.firstChild) {
            contentNode.removeChild(contentNode.firstChild);
        }

        let timeElapsed = (performance.now() - timerStart) / 1000;

        contentNode.appendChild(printPalindromesList(palindromes));
        contentNode.appendChild(printPalindromesList(palindromes));
        contentNode.appendChild(document
            .createElement("p")
            .appendChild(document.createTextNode(`Total time: ${timeElapsed} s. Count: ${palindromes.length}`))
            .parentNode);
    });
}

function printPalindromesList(palindromes: IPrimePalindrome[]): HTMLElement {

    let ul = document.createElement("ul");

    for (let palindrome of palindromes) {
        ul.appendChild(
            document.createElement("li")
                .appendChild(document.createTextNode(palindrome.toString()))
                .parentNode);
    }

    return ul;
}



