#!/usr/bin/env node
'use strict';

const circomlib = require('circomlib');
const snarkjs = require("snarkjs");
const bigInt = snarkjs.bigInt;
const crypto = require("crypto");
const F1Field = require("snarkjs/src/zqfield");
const babyJub = require("circomlib/src/babyjub");

const fs = require('fs');

/** Compute pedersen hash */
const pedersenHash = (data) => circomlib.babyJub.unpackPoint(circomlib.pedersenHash.hash(data))[0];

/** BigNumber to hex string of specified length */
function toHex(number, length = 32) {
    const str = number instanceof Buffer ? number.toString('hex') : bigInt(number).toString(16);
    return '0x' + str.padStart(length * 2, '0');
}

/** Generate random number of specified byte length */
const rbigint = (nbytes) => snarkjs.bigInt.leBuff2int(crypto.randomBytes(nbytes));

/**
 * Create deposit object from secret and nullifier
 */
/*
function createDeposit({nullifier, secret}) {
    const deposit = {nullifier, secret};
    deposit.preimage = Buffer.concat([deposit.nullifier.leInt2Buff(31), deposit.secret.leInt2Buff(31)]);

    const packedPoint = circomlib.pedersenHash.hash(deposit.preimage);
    const unpacked = circomlib.babyJub.unpackPoint(packedPoint);
    console.log('packedPoint', packedPoint);
    console.log('unpacked', unpacked);

    deposit.commitment = pedersenHash(deposit.preimage);
    if (deposit.commitment !== unpacked[0]) {
        console.error('commitment ', deposit.commitment, ' != unpacked ', unpacked[0]);
        throw new Error();
    }

    deposit.commitmentHex = toHex(deposit.commitment);
    deposit.nullifierHash = pedersenHash(deposit.nullifier.leInt2Buff(31));
    deposit.nullifierHex = toHex(deposit.nullifierHash);
    return deposit;
}

const deposit = createDeposit({
    nullifier: rbigint(31),
    secret: rbigint(31)
});


console.log('commitment', deposit.commitment, '\nnullifier', deposit.nullifier, '\nsecret', deposit.secret);
*/

{
    const numbers = [];
    // todo 100
    for (let i = 0; i < 50; i++) {
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
    // todo 10_000
    for (let i = 0; i < 100; i++) {
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
    let MulPointEscalarText = '';
    // todo 10_000
    for (let i = 0; i < 100; i++) {
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
    // todo 50_000
    for (let i = 0; i < 1000; i++) {
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
    // console.log(primaries);

    let F1FieldText = '';
    for (let i = 0; i < 1000; i++) {
        const id = Math.floor(Math.random() * primaries.length);
        const primary = primaries[id];
        const bigNumber = bigInt(primary);
        const f1 = new F1Field(bigNumber);

        // twoinv nqr t nqr_to_t
        const s = `${primary}\t${f1.twoinv}\t${f1.nqr}\t${f1.t}\t${f1.nqr_to_t}`;
        F1FieldText += s + '\n';
    }

    // save test data to file
    await fs.promises.writeFile('test-f1.tsv', F1FieldText);

    /*
    const nonResidueF2 = bigInt("21888242871839275222246405745257275088696311157297823662689037894645226208582");
    const nonResidueF6 = [ bigInt("9"), bigInt("1") ];

    let F2FieldText = '';
    for (let i = 0; i < 10; i++) {
        // number #1
        const id1 = Math.floor(Math.random() * primaries.length);
        const primary1 = primaries[id1];

        const big1 = bigInt(primary1);
        const f1 = new F1Field(big1);
        const f2a = new F2Field(f1, nonResidueF2);

        // number #2
        const id2 = Math.floor(Math.random() * primaries.length);
        const primary2 = primaries[id2];

        const big2 = bigInt(primary2);
        const f2 = new F1Field(big2);
        const f2b = new F2Field(f2, nonResidueF2);

        //
        const f2c = f2a.add(f2b);

        // twoinv nqr t nqr_to_t
        const s = `${primary1}\t${f1.twoinv}\t${f1.nqr}\t${f1.t}\t${f1.nqr_to_t}`;
        F2FieldText += s + '\n';
    }
    await fs.promises.writeFile('test-f2.tsv', F2FieldText);
    */
});

