#!/usr/bin/env node
'use strict';

const circomlib = require('circomlib');
const snarkjs = require("snarkjs");
const bigInt = snarkjs.bigInt;
const crypto = require("crypto");
const F1Field = require("snarkjs/src/zqfield");
const babyJub = require("circomlib/src/babyjub");
const createBlakeHash = require("blake-hash");

const fs = require('fs');

/** BigNumber to hex string of specified length */
function toHex(number, length = 32) {
    const str = number instanceof Buffer ? number.toString('hex') : bigInt(number).toString(16);
    return '0x' + str.padStart(length * 2, '0');
}

/** Generate random number of specified byte length */
const rbigint = (nbytes) => snarkjs.bigInt.leBuff2int(crypto.randomBytes(nbytes));

function padLeftZeros(idx, n) {
    let sidx = "" + idx;
    while (sidx.length < n) {
        sidx = "0" + sidx;
    }
    return sidx;
}

{
    const numbers = [];
    for (let i = 0; i < 100; i++) {
        const number = Math.floor(Math.random() * 1_000_000_000);
        numbers.push(bigInt(number));
    }

    let InverseText = '';
    for (let num1 of numbers) {
        for (let num2 of numbers) {
            const affine = num1.affine(num2);
            const inverse = num1.inverse(num2);

            const s = `${num1}\t${num2}\t${affine}\t${inverse}`;
            InverseText += s + '\n';
        }
    }

    fs.writeFileSync('test-affine-inverse.tsv', InverseText);
}

{
    let PointAddText = '';
    for (let i = 0; i < 10_000; i++) {
        if (i % 1000 === 999) {
            console.debug('generate add point', i);
        }

        const num1a = bigInt(Math.floor(Math.random() * 100_000));
        const num1b = bigInt(Math.floor(Math.random() * 100_000));

        const num2a = bigInt(Math.floor(Math.random() * 100_000));
        const num2b = bigInt(Math.floor(Math.random() * 100_000));

        const point1 = [num1a, num1b];
        const point2 = [num2a, num2b];

        const point3 = babyJub.addPoint(point1, point2);

        const s = `${num1a}\t${num1b}\t${num2a}\t${num2b}\t${point3[0]}\t${point3[1]}`;
        PointAddText += s + '\n';
    }

    fs.writeFileSync('test-add-point.tsv', PointAddText);
}

{
    const GENPOINT_PREFIX = "PedersenGenerator";
    let PackUnpackText = '';
    for (let pointIdx = 0; pointIdx < 100; pointIdx++) {
        let p = null;
        let h = null;
        let S = '';
        for (let tryIdx = 0; p == null; tryIdx++) {
            S = GENPOINT_PREFIX + "_" + padLeftZeros(pointIdx, 32) + "_" + padLeftZeros(tryIdx, 32);
            // noinspection JSCheckFunctionSignatures
            h = createBlakeHash("blake256").update(S).digest();
            h[31] = h[31] & 0xBF;  // Set 255th bit to 0 (256th is the signal and 254th is the last possible bit to 1)
            p = babyJub.unpackPoint(h);
        }

        const repack = babyJub.packPoint(p);

        const hHex = toHex(h, h.length);
        const repackHex = toHex(repack, repack.length);
        if (hHex !== repackHex) {
            throw new Error("Can't repack point");
        }

        const basePoint = circomlib.pedersenHash.getBasePoint(pointIdx);
        const s = `${pointIdx}\t${S}\t${hHex}\t${p[0]}\t${p[1]}\t${basePoint[0]}\t${basePoint[1]}`;
        PackUnpackText += s + '\n';
    }

    fs.writeFileSync('test-pack-unpack.tsv', PackUnpackText);

}

{
    let MulPointEscalarText = '';
    for (let i = 0; i < 10_000; i++) {
        if (i % 1000 === 999) {
            console.debug('generate escalar tests', i);
        }

        const num1 = bigInt(Math.floor(Math.random() * 100_000));
        const num2 = bigInt(Math.floor(Math.random() * 100_000));
        const num3 = Math.floor(Math.random() * 100_000);

        const accP = [num1, num2];

        const point2 = babyJub.mulPointEscalar(accP, num3);

        const s = `${num1}\t${num2}\t${num3}\t${point2[0]}\t${point2[1]}`;
        MulPointEscalarText += s + '\n';
    }

    fs.writeFileSync('test-mul-escalar.tsv', MulPointEscalarText);
}

{
    let DepositText = '';
    for (let i = 0; i < 50_000; i++) {
        if (i % 1000 === 999) {
            console.debug('generate main test: pedersen hash', i);
        }

        const nullifier = rbigint(31);
        const secret = rbigint(31);

        const preimage = Buffer.concat([nullifier.leInt2Buff(31), secret.leInt2Buff(31)]);

        const packedPoint = circomlib.pedersenHash.hash(preimage);
        const unpacked = circomlib.babyJub.unpackPoint(packedPoint);

        const preimageHex = toHex(preimage, preimage.length);
        const packedPointHex = toHex(packedPoint, packedPoint.length);
        const s = `${preimageHex}\t${packedPointHex}\t${unpacked[0]}\t${unpacked[1]}`;
        DepositText += s + '\n';
    }

    fs.writeFileSync('test-pedersen-hash.tsv', DepositText);
}

async function readPrimaries() {
    const contents = await fs.promises.readFile('prime.txt', 'utf-8');
    return contents.split("\n").map(line => line.trimEnd()).filter(x => x !== '');
}

readPrimaries().then(async primaries => {
    let F1FieldText = '';
    for (let i = 0; i < 1000; i++) {
        if (i % 100 === 99) {
            console.debug('generate f1 fields', i);
        }

        const id = Math.floor(Math.random() * primaries.length);
        const primary = primaries[id];
        const bigNumber = bigInt(primary);
        const f1 = new F1Field(bigNumber);

        const s = `${primary}\t${f1.twoinv}\t${f1.nqr}\t${f1.t}\t${f1.nqr_to_t}`;
        F1FieldText += s + '\n';
    }

    // save test data to file
    await fs.promises.writeFile('test-f1.tsv', F1FieldText);
});
